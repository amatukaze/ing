using Sakuno.ING.Composition;
using Sakuno.ING.Game.Provider.Json.Converters;
using Sakuno.ING.Messaging;
using System.Collections.Specialized;
using System.Text.Json;
using System.Web;

namespace Sakuno.ING.Game.Provider;

[Export]
public sealed partial class GameProvider : IGameProvider
{
    private readonly JsonModelContext _jsonModelContext;

    public GameProvider()
    {
        var options = new JsonSerializerOptions()
        {
            Converters =
            {
                new IdentifierConverterFactory(),
                new UnequippedSlotItemGroupConverter(),
            },
        };

        _jsonModelContext = new(options);
    }

    private partial bool HandleApiMessageCore(ApiMessage message);

    private NameValueCollection ParseRequest(string request) => HttpUtility.ParseQueryString(request);

    private void CheckResultCode(int code)
    {
        if (code is not 1)
            throw new Exception();
    }
}
