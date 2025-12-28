using System.Collections.Specialized;
using System.Reactive;
using System.Text;
using System.Web;

namespace Sakuno.ING.Game.Provider;

[RegisterSingleton(Registration = RegistrationStrategy.SelfWithProxyFactory)]
internal partial class EventDispatcher : IServiceInitializable
{
    private readonly GameProvider _provider;

    public EventDispatcher(GameProvider gameProvider, IApiMessageProvider messageProvider)
    {
        _provider = gameProvider;

        messageProvider.ApiMessages.Subscribe(message =>
        {
            HandleApiMessageCore(message);

            _provider.OnCommitted(Unit.Default);
        });
    }

    private partial bool HandleApiMessageCore(ApiMessage message);

    private NameValueCollection ParseRequest(ReadOnlyMemory<byte> buffer) =>
        HttpUtility.ParseQueryString(Encoding.UTF8.GetString(buffer.Span));

    private void CheckResultCode(int code)
    {
        if (code is not 1)
            throw new Exception();
    }

    Task IServiceInitializable.InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
