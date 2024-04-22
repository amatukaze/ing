using Sakuno.ING.Composition;
using Sakuno.ING.Game.Provider.Json.Converters;
using System.Buffers;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Sakuno.ING.Game.Provider;

[Export]
public sealed partial class GameProvider : IGameProvider
{
    private readonly JsonModelContext _jsonModelContext;

    public GameProvider(IApiMessageProvider messageProvider)
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

        messageProvider.ApiMessages.Subscribe(message =>
        {
            HandleApiMessageCore(message);
        });
    }

    private partial bool HandleApiMessageCore(ApiMessage message);

    private NameValueCollection ParseRequest(ReadOnlySequence<byte> buffer) =>
        HttpUtility.ParseQueryString(Encoding.UTF8.GetString(buffer));

    private void CheckResultCode(int code)
    {
        if (code is not 1)
            throw new Exception();
    }
}
