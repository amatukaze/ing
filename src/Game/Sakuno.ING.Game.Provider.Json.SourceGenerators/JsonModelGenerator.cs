using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

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
        var modelDescriptionFilesProvider = context.AdditionalTextsProvider.
            Where(static file => string.Equals(Path.GetExtension(file.Path), ".txt", StringComparison.OrdinalIgnoreCase));

        var provider = modelDescriptionFilesProvider.Combine(modelDescriptionDirectoryProvider).Combine(context.CompilationProvider);

        context.RegisterSourceOutput(provider, (context, tuple) =>
        {
            var ((file, directory), compilation) = tuple;

            var className = Path.GetFileNameWithoutExtension(file.Path);
            var additionTextDirectory = Path.GetDirectoryName(file.Path)!;
            var subNamespace = additionTextDirectory == directory ? string.Empty : additionTextDirectory.Substring(directory.Length + 1).Replace(Path.PathSeparator, '.');

            var usings = new Dictionary<string, UsingDirectiveSyntax>();
            var implementations = new Dictionary<string, INamedTypeSymbol>();
            var properties = new List<MemberDeclarationSyntax>();
            var properties2 = new List<MemberDeclarationSyntax>();

            foreach (var lineInfo in file.GetText(context.CancellationToken)!.Lines)
            {
                if (lineInfo.Span.IsEmpty)
                    continue;

                var parts = lineInfo.ToString().Split(' ');

                if (parts[0] is "@using")
                {
                    usings[parts[1]] = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(parts[1]));
                    continue;
                }

                if (parts[0] is "@implements")
                {
                    var implementationSymbol = compilation.GetTypeByMetadataName(parts[1]);

                    if (implementationSymbol is null)
                    {
                        foreach (var @using in usings.Keys)
                        {
                            implementationSymbol = compilation.GetTypeByMetadataName($"{@using}.{parts[1]}");

                            if (implementationSymbol is not null)
                                break;
                        }

                        if (implementationSymbol is null)
                            throw new Exception();
                    }

                    implementations[implementationSymbol.Name] = implementationSymbol;
                    continue;
                }

                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(parts[0]), "api_" + parts[1])
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    );

                properties.Add(property);

                for (var i = 2; i < parts.Length; i++)
                {
                    var implParts = parts[i].Split('.');
                    var impl = implementations[implParts[0]];
                    var members = impl.GetMembers(implParts[1]);
                    var propertyType = SyntaxFactory.ParseTypeName((members[0] as IPropertySymbol)!.Type.ToDisplayString());
                    var property2 = SyntaxFactory.PropertyDeclaration(propertyType, implParts[1])
                        .WithExplicitInterfaceSpecifier(SyntaxFactory.ExplicitInterfaceSpecifier(SyntaxFactory.IdentifierName(implParts[0])))
                        .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName("api_" + parts[1])))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                    properties2.Add(property2);
                }
            }

            var @class = SyntaxFactory.ClassDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(properties.ToArray())
                .AddMembers(properties2.ToArray());

            if (implementations.Count > 0)
                @class = @class.AddBaseListTypes(implementations.Keys.Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(i))).ToArray());

            var fullNamespace = subNamespace.Length is 0 ? "Sakuno.ING.Game.Provider.Json" : $"Sakuno.ING.Game.Provider.Json.{subNamespace}";
            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(fullNamespace))
                .AddMembers(@class);
            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(usings.Values.ToArray())
                .AddMembers(@namespace)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true)))
                .NormalizeWhitespace();

            var outputFilename = subNamespace.Length is 0 ? $"{className}.g.cs" : $"{subNamespace}.{className}.g.cs";

            context.AddSource(outputFilename, SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        });
    }
}
