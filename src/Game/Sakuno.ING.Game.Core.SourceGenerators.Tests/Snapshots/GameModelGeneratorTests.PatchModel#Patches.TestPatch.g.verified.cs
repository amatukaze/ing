//HintName: Patches.TestPatch.g.cs
#nullable disable
using TestNamespace;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Patches;
public record TestPatch(TestId Id, int B) : ITestPatched, IIdentifiable<TestId>;