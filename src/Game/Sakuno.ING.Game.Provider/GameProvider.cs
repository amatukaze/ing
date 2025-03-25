using System.Buffers;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Game.Provider;

[Export]
public sealed partial class GameProvider : IGameProvider
{
    public GameProvider(IApiMessageProvider messageProvider)
    {
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
