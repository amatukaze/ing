using System.Runtime.CompilerServices;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() => VerifySourceGenerators.Initialize();
}
