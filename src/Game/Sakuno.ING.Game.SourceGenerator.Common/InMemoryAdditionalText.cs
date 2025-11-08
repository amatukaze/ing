using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sakuno.ING.Game.SourceGenerator.Common;

public sealed class InMemoryAdditionalText(string path, string content) : AdditionalText
{
    public override string Path { get; } = path;

    public override SourceText? GetText(CancellationToken cancellationToken = default) =>
        SourceText.From(content);
}
