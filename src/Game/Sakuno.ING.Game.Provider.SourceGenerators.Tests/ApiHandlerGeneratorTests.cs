namespace Sakuno.ING.Game.Provider.SourceGenerators.Tests;

public class ApiHandlerGeneratorTests
{
    [Fact]
    public Task MultipleApis()
    {
        var source = """
                     namespace Sakuno.ING.Game.Provider;

                     public partial class EventDispatcher
                     {
                         [Api("api1")]
                         [Api("api2")]
                         [Api("api3")]
                         private void HandleApis(string api) { }
                     }
                     """;

        return Util.Verify<ApiHandlerGenerator>(source);
    }

    [Fact]
    public Task ResponseOnlyApi()
    {
        var source = """
                     namespace Sakuno.ING.Game.Provider;

                     public partial class EventDispatcher
                     {
                         [Api("api1")]
                         private void HandleApi1(Api1Json response) { }
                     }
                     """;

        return Util.Verify<ApiHandlerGenerator>(source);
    }

    [Fact]
    public Task RequestOnlyApiUsingNameValueCollection()
    {
        var source = """
                     using System.Collections.Specialized;

                     namespace Sakuno.ING.Game.Provider;

                     public partial class EventDispatcher
                     {
                         [Api("api1")]
                         private void HandleApi1(NameValueCollection request) { }
                     }
                     """;

        return Util.Verify<ApiHandlerGenerator>(source);
    }

    [Fact]
    public Task RequestOnlyApiUsingFromRequest()
    {
        var source = """
                     namespace Sakuno.ING.Game.Provider;

                     public partial class EventDispatcher
                     {
                         [Api("api1")]
                         private void HandleApi1([FromRequest("id")] ShipId id, [FromRequest("name")] string name, [FromRequest("flag")] bool flag) { }

                         [Api("api2")]
                         private void HandleApi2([FromRequest("abc")] int number, [FromRequest("items")] int[] items, [FromRequest("ids")] SlotItemId[] ids) { }
                     }
                     """;

        return Util.Verify<ApiHandlerGenerator>(source);
    }

    [Fact]
    public Task UnsupportedFromRequestParameterType()
    {
        var source = """
                     namespace Sakuno.ING.Game.Provider;

                     public partial class EventDispatcher
                     {
                         [Api("api1")]
                         private void HandleApi1([FromRequest("date")] DateTime date) { }
                     }
                     """;

        return Util.Verify<ApiHandlerGenerator>(source);
    }
}
