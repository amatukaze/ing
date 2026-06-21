using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.SourceGenerators;

internal abstract record GameModelInfoResult
{
    public abstract AdditionalText File { get; init; }

    private GameModelInfoResult() { }

    public sealed record Ok(AdditionalText File, GameModelInfo Info) : GameModelInfoResult;

    public sealed record Error(AdditionalText File, Diagnostic Diagnostic) : GameModelInfoResult;
}
