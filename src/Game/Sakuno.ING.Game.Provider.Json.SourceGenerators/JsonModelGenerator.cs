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
                return Path.Join(result, "Metadata");

            throw new InvalidOperationException("Missing build build_property.ProjectDir");
        });
        var modelDescriptionFilesProvider = context.AdditionalTextsProvider.
            Where(static text => string.Equals(Path.GetExtension(text.Path), ".txt", StringComparison.OrdinalIgnoreCase));

        context.RegisterSourceOutput(modelDescriptionFilesProvider.Combine(modelDescriptionDirectoryProvider), (context, tuple) =>
        {
            var (file, directory) = tuple;

            var className = Path.GetFileNameWithoutExtension(file.Path);
            var additionTextDirectory = Path.GetDirectoryName(file.Path)!;
            var subNamespace = additionTextDirectory == directory ? string.Empty : additionTextDirectory.Substring(directory.Length + 1).Replace(Path.PathSeparator, '.');

            var usings = new List<UsingDirectiveSyntax>();
            var properties = new List<MemberDeclarationSyntax>();

            using var reader = File.OpenText(file.Path);

            while (true)
            {
                var line = reader.ReadLine();
                if (line is null)
                    break;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(' ');

                if (parts[0] is "@using")
                {
                    usings.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(parts[1])));
                    continue;
                }

                var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(parts[0]), "api_" + parts[1])
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    );

                properties.Add(property);
            }

            var @class = SyntaxFactory.ClassDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(properties.ToArray());
            var fullNamespace = subNamespace.Length is 0 ? "Sakuno.ING.Game.Provider.Json" : $"Sakuno.ING.Game.Provider.Json.{subNamespace}";
            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(fullNamespace))
                .AddMembers(@class);
            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(usings.ToArray())
                .AddMembers(@namespace)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true)))
                .NormalizeWhitespace();

            var outputFilename = subNamespace.Length is 0 ? $"{className}.g.cs" : $"{subNamespace}.{className}.g.cs";

            context.AddSource(outputFilename, SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        });
    }
}
