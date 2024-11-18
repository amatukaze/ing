using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class JsonModelGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modelDescriptionDirectoryProvider = context.AnalyzerConfigOptionsProvider.Select(static (context, _) =>
        {
            if (context.GlobalOptions.TryGetValue("build_property.ProjectDir", out var result))
                return Path.Combine(result, "Metadata");

            throw new InvalidOperationException("Missing build build_property.ProjectDir");
        });
        var modelDescriptionFileProvider = context.AdditionalTextsProvider.
            Where(static file => string.Equals(Path.GetExtension(file.Path), ".jsondesc", StringComparison.OrdinalIgnoreCase));

        var modelInfoProvider = modelDescriptionFileProvider.Combine(modelDescriptionDirectoryProvider).Select(static (tuple, cancellationToken) =>
        {
            var (file, projectDirectory) = tuple;
            var className = Path.GetFileNameWithoutExtension(file.Path);
            var fileDirectory = Path.GetDirectoryName(file.Path)!;
            var subNamespace = fileDirectory == projectDirectory ? string.Empty : fileDirectory.Substring(projectDirectory.Length + 1).Replace(Path.PathSeparator, '.');

            return JsonModelInfo.Create(file, className, subNamespace, cancellationToken);
        });

        var provider = modelInfoProvider.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(provider, static (context, tuple) =>
        {
            var (info, compilation) = tuple;

            var usings = info.Usings.ToDictionary(u => u, u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)), StringComparer.Ordinal);
            var properties = info.Properties.Select(p => SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(p.Type), "api_" + p.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                )).ToArray();

            var implementations = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);
            var mappingProperties = new List<MemberDeclarationSyntax>();

            foreach (var implementation in info.Implementations)
            {
                var implementationSymbol = compilation.GetTypeByMetadataName(implementation);

                if (implementationSymbol is null)
                {
                    foreach (var @using in usings.Keys)
                    {
                        implementationSymbol = compilation.GetTypeByMetadataName($"{@using}.{implementation}");

                        if (implementationSymbol is not null)
                            break;
                    }

                    if (implementationSymbol is null)
                        throw new Exception();
                }

                implementations[implementationSymbol.Name] = implementationSymbol;
            }

            if (info.IdType is not null)
                mappingProperties.Add(SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(info.IdType), "Id")
                    .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName($"IIdentifiable<{info.IdType}>")))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName("api_id")))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            foreach (var (implementation, implementationProperty, property) in info.Mappings)
            {
                var members = implementations[implementation].GetMembers(implementationProperty);
                var propertyType = SyntaxFactory.ParseTypeName((members[0] as IPropertySymbol)!.Type.ToDisplayString());

                mappingProperties.Add(SyntaxFactory.PropertyDeclaration(propertyType, implementationProperty)
                    .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName(implementation)))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName("api_" + property)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            var @class = SyntaxFactory.ClassDeclaration(info.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(properties.ToArray())
                .AddMembers(mappingProperties.ToArray());

            if (implementations.Count > 0)
                @class = @class.AddBaseListTypes(implementations.Keys.Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(i))).ToArray());

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
