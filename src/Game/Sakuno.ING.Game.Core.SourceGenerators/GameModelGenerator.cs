using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Sakuno.ING.Game.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class GameModelGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modelDescriptionDirectoryProvider = context.AnalyzerConfigOptionsProvider.Select(static (context, _) =>
        {
            if (context.GlobalOptions.TryGetValue("build_property.ProjectDir", out var result))
                return Path.Combine(result, "Models", "Metadata");

            throw new InvalidOperationException("Missing build build_property.ProjectDir");
        });
        var modelDescriptionFileProvider = context.AdditionalTextsProvider.
            Where(static file => string.Equals(Path.GetExtension(file.Path), ".modeldesc", StringComparison.OrdinalIgnoreCase));

        var modelInfoProvider = modelDescriptionFileProvider.Combine(modelDescriptionDirectoryProvider).Select(static (tuple, cancellationToken) =>
        {
            var (file, projectDirectory) = tuple;
            var className = Path.GetFileNameWithoutExtension(file.Path);
            var fileDirectory = Path.GetDirectoryName(file.Path)!;
            var subNamespace = fileDirectory == projectDirectory ? string.Empty : fileDirectory.Substring(projectDirectory.Length + 1).Replace(Path.PathSeparator, '.');

            return GameModelInfo.Create(file, className, subNamespace, cancellationToken);
        });

        var propertiesProvider = modelInfoProvider
            .SelectMany(static (info, _) => info.Properties)
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

        var provider = modelInfoProvider.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(provider, static (context, tuple) =>
        {
            var (info, compilation) = tuple;
            var members = new List<MemberDeclarationSyntax>();

            var usings = info.Usings.ToDictionary(u => u, u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)), StringComparer.Ordinal);

            members.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(info.IdType), "Id")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))));

            foreach (var (type, name) in info.Properties)
            {
                var fieldName = $"_{char.ToLowerInvariant(name[0])}{name.Substring(1)}";
                var field = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName(type),
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(fieldName))))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName(fieldName)))
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("SetField"),
                                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] {
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(fieldName)).WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value")),
                                    SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("PropertyNames"), SyntaxFactory.IdentifierName(name))),
                                })))))
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                members.Add(field);
                members.Add(property);
            }

            members.Add(SyntaxFactory.ConstructorDeclaration(info.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("id")).WithType(SyntaxFactory.ParseTypeName(info.IdType)))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.List(new[]
                {
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("Id"), SyntaxFactory.IdentifierName("id"))),
                }))));

            members.Add(SyntaxFactory.ConstructorDeclaration(info.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithInitializer(SyntaxFactory.ConstructorInitializer(SyntaxKind.ThisConstructorInitializer,
                    SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("raw"), SyntaxFactory.IdentifierName("Id")))))))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.List(new[]
                {
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("Update"),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("raw")))))),
                }))));

            members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(info.ClassName), "Create")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("raw")))), null)))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            members.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Update")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("raw")).WithType(SyntaxFactory.ParseTypeName(info.RawType)))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.List(GenerateUpdateMethodBody(info, compilation, context.CancellationToken).ToArray()))));

            var @class = SyntaxFactory.ClassDeclaration(info.ClassName)
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BindableObject")),
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"IModel<{info.ClassName}, {info.IdType}, {info.RawType}>")))
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
        });
    }

    private static IEnumerable<StatementSyntax> GenerateUpdateMethodBody(GameModelInfo info, Compilation compilation, CancellationToken cancellationToken)
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
                yield break;
        }

        foreach (var (_, propertyName) in info.Properties)
        {
            var propertyMetadata = rawSymbol.GetMembers(propertyName).SingleOrDefault();
            if (propertyMetadata is null)
                continue;

            yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(propertyName),
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("raw"), SyntaxFactory.IdentifierName(propertyName))));
        }
    }
}
