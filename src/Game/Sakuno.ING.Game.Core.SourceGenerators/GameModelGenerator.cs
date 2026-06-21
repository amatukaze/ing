using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class GameModelGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor ModelDescriptionParseErrorDescriptor = new(
        "INGGC001",
        "Model description parse error",
        "Failed to parse model description: {0}",
        "GameModelGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor PatchPropertyNotDefinedDescriptor = new(
        "INGGC002",
        "Patch property not defined",
        "Property '{0}' declared in patch '{1}' is not defined",
        "GameModelGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modelDescriptionDirectoryProvider = context.AnalyzerConfigOptionsProvider.Select(static (context, _) =>
        {
            if (context.GlobalOptions.TryGetValue("build_property.ProjectDir", out var result))
                return Path.Combine(result, "Models", "Metadata");

            throw new InvalidOperationException("Missing build build_property.ProjectDir");
        });
        var modelDescriptionFileProvider = context.AdditionalTextsProvider.Where(static file =>
            string.Equals(Path.GetExtension(file.Path), ".modeldesc", StringComparison.OrdinalIgnoreCase));

        var modelInfoProvider = modelDescriptionFileProvider.Combine(modelDescriptionDirectoryProvider).Select(static (tuple, cancellationToken) =>
        {
            var (file, projectDirectory) = tuple;
            var className = Path.GetFileNameWithoutExtension(file.Path);
            var fileDirectory = Path.GetDirectoryName(file.Path)!;
            var subNamespace = fileDirectory == projectDirectory ? string.Empty : fileDirectory.Substring(projectDirectory.Length + 1).Replace(Path.PathSeparator, '.');

            GameModelInfoResult result;
            try
            {
                result = new GameModelInfoResult.Ok(file, GameModelInfo.Create(file, className, subNamespace, cancellationToken));
            }
            catch (InvalidOperationException ex)
            {
                var diagnostic = Diagnostic.Create(ModelDescriptionParseErrorDescriptor, Location.Create(file.Path, default, default), ex.Message);
                result = new GameModelInfoResult.Error(file, diagnostic);
            }

            return result;
        });

        var propertiesProvider = modelInfoProvider
            .SelectMany(static (result, _) => result is GameModelInfoResult.Ok ok ? ok.Info.Properties : [])
            .Select(static (tuple, _) => tuple.Name).Collect();

        context.RegisterSourceOutput(propertiesProvider, static (context, propertyNames) =>
        {
            var members = propertyNames.Distinct(StringComparer.Ordinal)
                .Select(name => SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("PropertyChangedEventArgs"),
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(name)
                            .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.ImplicitObjectCreationExpression()
                                .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(name)))))))))))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)))
                .ToArray();

            var @class = SyntaxFactory.ClassDeclaration("PropertyNames")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddMembers(members.ToArray());

            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game.Models"))
                .AddMembers(@class);
            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.ComponentModel")))
                .AddMembers(@namespace)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource("PropertyNames.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText(context.CancellationToken));
        });

        var modelWithUpdateInfoProvider = modelInfoProvider.Combine(context.CompilationProvider).Select(static (tuple, cancellationToken) =>
        {
            var (result, compilation) = tuple;
            if (result is GameModelInfoResult.Error)
                return new GameModelUpdateInfo(result, ImmutableArray<string>.Empty);

            var updateablePropertyNames = GetUpdateablePropertyNames(((GameModelInfoResult.Ok)result).Info, compilation);
            return new GameModelUpdateInfo(result, updateablePropertyNames);
        });

        context.RegisterSourceOutput(modelWithUpdateInfoProvider, static (context, updateInfo) =>
        {
            if (updateInfo.Result is GameModelInfoResult.Error error)
            {
                context.ReportDiagnostic(error.Diagnostic);
                return;
            }

            var result = (GameModelInfoResult.Ok)updateInfo.Result;
            var info = result.Info;
            if (info.PatchBaseType is not null)
            {
                var propertyTypeMap = info.Properties.ToDictionary(p => p.Name, p => p.Type, StringComparer.Ordinal);
                var validPatches = new List<(string InterfaceName, IReadOnlyList<string> PropertyNames)>();

                foreach (var (patchClassName, patchPropertyNames) in info.Patches)
                {
                    var hasPatchError = false;
                    foreach (var propertyName in patchPropertyNames)
                    {
                        if (propertyTypeMap.ContainsKey(propertyName))
                            continue;

                        context.ReportDiagnostic(Diagnostic.Create(PatchPropertyNotDefinedDescriptor, Location.Create(result.File.Path, default, default), propertyName, patchClassName));
                        hasPatchError = true;
                    }

                    if (!hasPatchError)
                        validPatches.Add((patchClassName, patchPropertyNames));
                }

                info = info with { Patches = validPatches };
            }

            var members = new List<MemberDeclarationSyntax>();

            var usings = info.Usings.ToDictionary(u => u, u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)), StringComparer.Ordinal);
            var originalUsings = usings.Values.ToList();

            if (info.Patches.Count > 0)
            {
                if (!usings.ContainsKey("Sakuno.ING.Game"))
                    usings.Add("Sakuno.ING.Game", SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Sakuno.ING.Game")));

                if (!usings.ContainsKey("Sakuno.ING.Game.Patches"))
                    usings.Add("Sakuno.ING.Game.Patches", SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Sakuno.ING.Game.Patches")));
            }

            members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(info.IdType), "Id")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));

            foreach (var (type, name) in info.Properties)
            {
                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("SetField"),
                                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList([
                                    SyntaxFactory.Argument(SyntaxFactory.FieldExpression()).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value")),
                                    SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("PropertyNames"), SyntaxFactory.IdentifierName(name)))
                                ])))))
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                members.Add(property);
            }

            members.Add(SyntaxFactory.ConstructorDeclaration(info.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("id")).WithType(SyntaxFactory.ParseTypeName(info.IdType)))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.List([
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("Id"), SyntaxFactory.IdentifierName("id"))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("CreateCore"), SyntaxFactory.ArgumentList())),
                ]))));

            members.Add(SyntaxFactory.ConstructorDeclaration(info.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithInitializer(SyntaxFactory.ConstructorInitializer(SyntaxKind.ThisConstructorInitializer,
                    SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("raw"), SyntaxFactory.IdentifierName("Id")))))))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.List([
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("Update"),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("raw")))))),
                ]))));

            members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(info.ClassName), "Create")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("raw")))), null)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Update")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.List(
                    GenerateUpdateMethodBody(updateInfo.UpdateablePropertyNames)
                        .Append(SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("UpdateCore"),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("raw")))))))
                        .ToArray()))));

            if (info.PatchBaseType is not null)
            {
                var dispatchVariableName = info.Patches.Select(p => $"{p.InterfaceName.TrimStart('I')}Value").ToList();

                var switchSections = info.Patches.Select((patch, index) =>
                {
                    var variableName = dispatchVariableName[index];
                    return SyntaxFactory.SwitchSection()
                        .AddLabels(SyntaxFactory.CasePatternSwitchLabel(
                            SyntaxFactory.DeclarationPattern(
                                SyntaxFactory.ParseTypeName(patch.InterfaceName),
                                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(variableName))),
                            SyntaxFactory.Token(SyntaxKind.ColonToken)))
                        .AddStatements(
                            SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("Patch"),
                                SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(variableName)))))),
                            SyntaxFactory.BreakStatement());
                }).ToArray();

                members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Patch")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("patch")).WithType(SyntaxFactory.ParseTypeName(info.PatchBaseType)))
                    .WithBody(SyntaxFactory.Block(SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.SwitchStatement(SyntaxFactory.IdentifierName("patch"), SyntaxFactory.List(switchSections))))));

                var patchTypeParameter = SyntaxFactory.TypeParameter("TPatch");
                var patchConstraint = SyntaxFactory.TypeParameterConstraintClause("TPatch")
                    .AddConstraints(SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName(info.PatchBaseType)));

                members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Patch")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddTypeParameterListParameters(patchTypeParameter)
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("patch")).WithType(SyntaxFactory.ParseTypeName("TPatch")))
                    .AddConstraintClauses(patchConstraint)
                    .WithBody(SyntaxFactory.Block(SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("Patch"),
                            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName(info.PatchBaseType), SyntaxFactory.IdentifierName("patch")))))))))));
            }

            foreach (var (patchInterfaceName, patchPropertyNames) in info.Patches)
            {
                var patchStatements = patchPropertyNames
                    .Select(propertyName => SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(propertyName),
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("patch"), SyntaxFactory.IdentifierName(propertyName)))))
                    .ToArray();

                members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Patch")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("patch")).WithType(SyntaxFactory.ParseTypeName(patchInterfaceName)))
                    .WithBody(SyntaxFactory.Block(SyntaxFactory.List(patchStatements))));
            }

            members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "CreateCore")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "UpdateCore")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            var baseTypes = new List<BaseTypeSyntax>()
            {
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BindableObject")),
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"IModel<{info.ClassName}, {info.IdType}, {info.RawType}>"))
            };

            if (info.PatchBaseType is not null)
                baseTypes.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"IPatchable<{info.IdType}, {info.PatchBaseType}>")));

            var @class = SyntaxFactory.ClassDeclaration(info.ClassName)
                .AddBaseListTypes(baseTypes.ToArray())
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.SealedKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(members.ToArray());

            var fullNamespace = string.IsNullOrWhiteSpace(info.Subnamespace) ? "Sakuno.ING.Game.Models" : $"Sakuno.ING.Game.Models.{info.Subnamespace}";
            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(fullNamespace))
                .AddMembers(@class);
            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(usings.Values.ToArray())
                .AddMembers(@namespace)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true)))
                .NormalizeWhitespace();

            var outputFilename = string.IsNullOrWhiteSpace(info.Subnamespace) ? $"{info.ClassName}.g.cs" : $"{info.Subnamespace}.{info.ClassName}.g.cs";

            context.AddSource(outputFilename, SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText(context.CancellationToken));

            if (info.PatchBaseType is not null)
            {
                var propertyTypeMap = info.Properties.ToDictionary(p => p.Name, p => p.Type, StringComparer.Ordinal);

                foreach (var (patchClassName, patchPropertyNames) in info.Patches)
                {
                    var patchParameters = new List<ParameterSyntax>()
                    {
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("Id")).WithType(SyntaxFactory.ParseTypeName(info.IdType))
                    };

                    foreach (var propertyName in patchPropertyNames)
                    {
                        var propertyType = propertyTypeMap[propertyName];
                        patchParameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(propertyName)).WithType(SyntaxFactory.ParseTypeName(propertyType)));
                    }

                    var record = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), patchClassName)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddParameterListParameters(patchParameters.ToArray())
                        .AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(info.PatchBaseType)),
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"IIdentifiable<{info.IdType}>")))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                    var patchNamespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game.Patches"))
                        .AddMembers(record);

                    var patchUsings = originalUsings.ToList();
                    patchUsings.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Sakuno.ING.Game.Models")));

                    var patchCompilationUnit = SyntaxFactory.CompilationUnit()
                        .AddUsings(patchUsings.ToArray())
                        .AddMembers(patchNamespace)
                        .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true)))
                        .NormalizeWhitespace();

                    context.AddSource($"Patches.{patchClassName}.g.cs", SyntaxFactory.SyntaxTree(patchCompilationUnit, encoding: Encoding.UTF8).GetText(context.CancellationToken));
                }
            }
        });
    }

    private static ImmutableArray<string> GetUpdateablePropertyNames(GameModelInfo info, Compilation compilation)
    {
        var rawSymbol = compilation.GetTypeByMetadataName(info.RawType);

        if (rawSymbol is null)
        {
            foreach (var @using in info.Usings)
            {
                rawSymbol = compilation.GetTypeByMetadataName($"{@using}.{info.RawType}");

                if (rawSymbol is not null)
                    break;
            }

            if (rawSymbol is null)
                return ImmutableArray<string>.Empty;
        }

        return info.Properties
            .Where(property => rawSymbol.GetMembers(property.Name).Any())
            .Select(property => property.Name)
            .ToImmutableArray();
    }

    private static IEnumerable<StatementSyntax> GenerateUpdateMethodBody(ImmutableArray<string> updateablePropertyNames)
    {
        foreach (var propertyName in updateablePropertyNames)
        {
            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(propertyName),
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("raw"), SyntaxFactory.IdentifierName(propertyName))));
        }
    }
}
