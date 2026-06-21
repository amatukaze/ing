using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators;

internal readonly record struct JsonModelGenerationInfo(
    JsonModelInfoResult Result,
    ImmutableArray<string> Implementations,
    ImmutableDictionary<string, string> MappingPropertyTypes,
    ImmutableArray<Diagnostic> Diagnostics);
