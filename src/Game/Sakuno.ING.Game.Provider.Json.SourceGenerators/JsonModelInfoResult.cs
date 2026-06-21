using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators;

internal abstract record JsonModelInfoResult
{
    public abstract AdditionalText File { get; init; }

    private JsonModelInfoResult() { }

    public sealed record Ok(AdditionalText File, JsonModelInfo Info) : JsonModelInfoResult;

    public sealed record Error(AdditionalText File, Diagnostic Diagnostic) : JsonModelInfoResult;
}
