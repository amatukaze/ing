using System.Collections.Immutable;

namespace Sakuno.ING.Game.SourceGenerators;

internal readonly record struct GameModelUpdateInfo(GameModelInfoResult Result, ImmutableArray<string> UpdateablePropertyNames);
