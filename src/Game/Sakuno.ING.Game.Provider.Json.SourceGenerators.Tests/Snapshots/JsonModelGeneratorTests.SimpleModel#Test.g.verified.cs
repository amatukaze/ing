//HintName: Test.g.cs
#nullable disable
using TestNamespace;

namespace Sakuno.ING.Game.Provider.Json;
public partial class Test : ITestUpdated
{
    public int api_a { get; set; }
    public int api_b { get; set; }
    public int api_c { get; set; }

    int ITestUpdated.A => api_a;

    int ITestUpdated.B => api_b;
}