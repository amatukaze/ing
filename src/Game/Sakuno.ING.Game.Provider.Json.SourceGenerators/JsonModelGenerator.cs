using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class JsonModelGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor ModelDescriptionParseErrorDescriptor = new(
        "INGJSON001",
        "JSON model description parse error",
        "Failed to parse JSON model description: {0}",
        "JsonModelGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ImplementationNotFoundDescriptor = new(
        "INGJSON002",
        "Implementation not found",
        "Implementation type '{0}' could not be found",
        "JsonModelGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MappingPropertyNotFoundDescriptor = new(
        "INGJSON003",
        "Mapping property not found",
        "Property '{0}' could not be found on implementation type '{1}'",
        "JsonModelGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modelDescriptionDirectoryProvider = context.AnalyzerConfigOptionsProvider.Select(static (context, _) =>
        {
            if (context.GlobalOptions.TryGetValue("build_property.ProjectDir", out var result))
                return Path.Combine(result, "Metadata");

            throw new InvalidOperationException("Missing build build_property.ProjectDir");
        });
        var modelDescriptionFileProvider = context.AdditionalTextsProvider.Where(static file =>
            string.Equals(Path.GetExtension(file.Path), ".jsondesc", StringComparison.OrdinalIgnoreCase));

        var modelInfoProvider = modelDescriptionFileProvider.Combine(modelDescriptionDirectoryProvider).Select(static (tuple, cancellationToken) =>
        {
            var (file, projectDirectory) = tuple;
            var className = Path.GetFileNameWithoutExtension(file.Path);
            var fileDirectory = Path.GetDirectoryName(file.Path)!;
            var subNamespace = fileDirectory == projectDirectory ? string.Empty : fileDirectory.Substring(projectDirectory.Length + 1).Replace(Path.PathSeparator, '.');

            JsonModelInfoResult result;
            try
            {
                result = new JsonModelInfoResult.Ok(file, JsonModelInfo.Create(file, className, subNamespace, cancellationToken));
            }
            catch (InvalidOperationException ex)
            {
                var diagnostic = Diagnostic.Create(ModelDescriptionParseErrorDescriptor, Location.Create(file.Path, default, default), ex.Message);
                result = new JsonModelInfoResult.Error(file, diagnostic);
            }

            return result;
        });

        var modelWithGenerationInfoProvider = modelInfoProvider.Combine(context.CompilationProvider).Select(static (tuple, cancellationToken) =>
        {
            var (result, compilation) = tuple;
            if (result is JsonModelInfoResult.Error)
                return new JsonModelGenerationInfo(result, ImmutableArray<string>.Empty, ImmutableDictionary<string, string>.Empty, ImmutableArray<Diagnostic>.Empty);

            var ok = (JsonModelInfoResult.Ok)result;
            var info = ok.Info;
            var usings = info.Usings;
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            var validImplementations = new List<string>();
            var mappingPropertyTypes = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var implementation in info.Implementations)
            {
                var implementationSymbol = compilation.GetTypeByMetadataName(implementation);

                if (implementationSymbol is null)
                {
                    foreach (var @using in usings)
                    {
                        implementationSymbol = compilation.GetTypeByMetadataName($"{@using}.{implementation}");

                        if (implementationSymbol is not null)
                            break;
                    }
                }

                if (implementationSymbol is null)
                {
                    diagnostics.Add(Diagnostic.Create(ImplementationNotFoundDescriptor, Location.Create(ok.File.Path, default, default), implementation));
                    continue;
                }

                validImplementations.Add(implementation);

                foreach (var (mappingImplementation, implementationProperty, property) in info.Mappings)
                {
                    if (mappingImplementation != implementation)
                        continue;

                    var members = implementationSymbol.GetMembers(implementationProperty);
                    if (members.IsEmpty)
                    {
                        diagnostics.Add(Diagnostic.Create(MappingPropertyNotFoundDescriptor, Location.Create(ok.File.Path, default, default), implementationProperty, implementation));
                        continue;
                    }

                    var propertySymbol = members[0] as IPropertySymbol;

                    if (propertySymbol is null)
                    {
                        diagnostics.Add(Diagnostic.Create(MappingPropertyNotFoundDescriptor, Location.Create(ok.File.Path, default, default), implementationProperty, implementation));
                        continue;
                    }

                    mappingPropertyTypes[$"{implementation}.{implementationProperty}"] = propertySymbol.Type.ToDisplayString();
                }
            }

            return new JsonModelGenerationInfo(
                result,
                validImplementations.ToImmutableArray(),
                mappingPropertyTypes.ToImmutable(),
                diagnostics.ToImmutable());
        });

        context.RegisterSourceOutput(modelWithGenerationInfoProvider, static (context, generationInfo) =>
        {
            foreach (var diagnostic in generationInfo.Diagnostics)
                context.ReportDiagnostic(diagnostic);

            if (generationInfo.Result is JsonModelInfoResult.Error error)
            {
                context.ReportDiagnostic(error.Diagnostic);
                return;
            }

            var ok = (JsonModelInfoResult.Ok)generationInfo.Result;
            var info = ok.Info;

            var usings = info.Usings.ToDictionary(u => u, u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)), StringComparer.Ordinal);
            var properties = info.Properties.Select(p => SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(p.Type), "api_" + p.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                )).ToArray();

            var mappingProperties = new List<MemberDeclarationSyntax>();

            if (info.IdType is not null)
                mappingProperties.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(info.IdType), "Id")
                    .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName($"IIdentifiable<{info.IdType}>")))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName("api_id")))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            foreach (var (implementation, implementationProperty, property) in info.Mappings)
            {
                if (!generationInfo.MappingPropertyTypes.TryGetValue($"{implementation}.{implementationProperty}", out var propertyType))
                    continue;

                mappingProperties.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyType), implementationProperty)
                    .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName(implementation)))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName("api_" + property)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            var @class = SyntaxFactory.ClassDeclaration(info.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(properties.ToArray())
                .AddMembers(mappingProperties.ToArray());

            if (generationInfo.Implementations.Length > 0)
                @class = @class.AddBaseListTypes(generationInfo.Implementations.Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(i))).ToArray());

            var fullNamespace = string.IsNullOrWhiteSpace(info.Subnamespace) ? "Sakuno.ING.Game.Provider.Json" : $"Sakuno.ING.Game.Provider.Json.{info.Subnamespace}";
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
}
