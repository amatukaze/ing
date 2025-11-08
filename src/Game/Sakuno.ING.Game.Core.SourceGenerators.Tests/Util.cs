using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sakuno.ING.Game.SourceGenerator.Common;
using Sakuno.ING.Game.SourceGenerators;

namespace Sakuno.ING.Game.Core.SourceGenerators.Tests;

internal static class Util
{
    public static Task VerifyGameModel(string name, string description, string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

        var compilation = CSharpCompilation.Create("GameCoreTests",
            [syntaxTree],
            references,
            new(OutputKind.DynamicallyLinkedLibrary));

        var generator = new GameModelGenerator();
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
