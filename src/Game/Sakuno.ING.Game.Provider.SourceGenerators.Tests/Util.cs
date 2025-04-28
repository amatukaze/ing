using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Sakuno.ING.Game.Provider.SourceGenerators.Tests;

internal static class Util
{
    public static string? GetGeneratedOutput<T>(string source) where T : IIncrementalGenerator, new()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Concat([
                MetadataReference.CreateFromFile(typeof(GameProvider).Assembly.Location),
            ]);

        var compilation = CSharpCompilation.Create("GameProviderTests",
            [syntaxTree],
            references,
            new(OutputKind.DynamicallyLinkedLibrary));

        var generator = new T();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        Assert.Empty(diagnostics.Where(d => d.Severity is DiagnosticSeverity.Error));

        return outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString();
    }
}
