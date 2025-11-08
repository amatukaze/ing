using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sakuno.ING.Game.SourceGenerator.Common;

public class DictionaryAnalyzerOptions(Dictionary<string, string> options) : AnalyzerConfigOptions
{
    public override IEnumerable<string> Keys => options.Keys;

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        options.TryGetValue(key, out value);
}
