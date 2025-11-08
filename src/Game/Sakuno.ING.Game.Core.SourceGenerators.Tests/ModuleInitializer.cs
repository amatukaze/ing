using System.Runtime.CompilerServices;

namespace Sakuno.ING.Game.Core.SourceGenerators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() => VerifySourceGenerators.Initialize();
}
