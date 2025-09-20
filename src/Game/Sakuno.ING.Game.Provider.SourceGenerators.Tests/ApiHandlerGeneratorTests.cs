namespace Sakuno.ING.Game.Provider.SourceGenerators.Tests;

public class ApiHandlerGeneratorTests
{
    [Fact]
    public Task ResponseOnlyApi()
    {
        var source = """
                     namespace Sakuno.ING.Game.Provider;

                     public partial class GameProvider
                     {
                         [Api("api1")]
                         private void HandleApi1(Api1Json response) { }
                     }
                     """;

        return Util.Verify<ApiHandlerGenerator>(source);
    }
}
