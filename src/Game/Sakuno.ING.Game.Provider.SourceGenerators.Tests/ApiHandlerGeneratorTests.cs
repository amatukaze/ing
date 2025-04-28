namespace Sakuno.ING.Game.Provider.SourceGenerators.Tests;

public class ApiHandlerGeneratorTests
{
    [Fact]
    public void ResponseOnlyApi()
    {
        var source = """
                     namespace Sakuno.ING.Game.Provider;

                     public partial class GameProvider
                     {
                         [Api("api1")]
                         private void HandleApi1(Api1Json response) { }
                     }
                     """;

        var result = Util.GetGeneratedOutput<ApiHandlerGenerator>(source);

        var expected = """
                       #nullable enable
                       using System;
                       using System.Text.Json;
                       using Sakuno.ING.Game;

                       namespace Sakuno.ING.Game.Provider;
                       partial class GameProvider
                       {
                           private partial bool HandleApiMessageCore(ApiMessage message)
                           {
                               switch (message.Api)
                               {
                                   case "api1":
                                   {
                                       var reader = new Utf8JsonReader(message.Response);
                                       var response = JsonSerializer.Deserialize(ref reader, JsonModelContext.Default.SvDataApi1Json)!;
                                       CheckResultCode(response.api_result);
                                       HandleApi1(response.api_data);
                                       return true;
                                   }
                               }

                               return false;
                           }
                       }
                       """;

        Assert.Equal(expected, result);
    }
}
