using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sakuno.ING.Game.SourceGenerator.Common;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators.Tests;

internal static class Util
{
    public static Task VerifyJsonModel(string name, string description, string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

        var compilation = CSharpCompilation.Create("GameProviderJsonTests",
            [syntaxTree],
            references,
            new(OutputKind.DynamicallyLinkedLibrary));

        var generator = new JsonModelGenerator();
        var options = new DictionaryAnalyzerOptions(new Dictionary<string, string>()
        {
            ["build_property.ProjectDir"] = "ProjectRoot",
        });
        var driver = CSharpGeneratorDriver.Create(generator)
            .WithUpdatedAnalyzerConfigOptions(new MockAnalyzerConfigOptionsProvider(options))
            .AddAdditionalTexts([new InMemoryAdditionalText(name, description)])
            .RunGenerators(compilation);

        return Verifier.Verify(driver).UseDirectory("Snapshots");
    }
}
