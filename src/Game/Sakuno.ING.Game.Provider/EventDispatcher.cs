using System.Collections.Specialized;
using System.Reactive;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Sakuno.ING.Game.Provider;

[RegisterSingleton(Registration = RegistrationStrategy.SelfWithProxyFactory)]
internal partial class EventDispatcher : IServiceInitializable
{
    private readonly IGameProviderSource _provider;
    private readonly IMasterDataSnapshotService _masterDataSnapshotService;
    private readonly IPlayerDataSnapshotService _playerDataSnapshotService;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(IGameProviderSource gameProvider,
        IMasterDataSnapshotService masterDataSnapshotService,
        IPlayerDataSnapshotService playerDataSnapshotService,
        IApiMessageProvider messageProvider,
        ILogger<EventDispatcher> logger)
    {
        _provider = gameProvider;
        _masterDataSnapshotService = masterDataSnapshotService;
        _playerDataSnapshotService = playerDataSnapshotService;
        _logger = logger;

        messageProvider.ApiMessages.Subscribe(message =>
        {
            try
            {
                HandleApiMessageCore(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle API message {Api}", message.Api);
            }
            finally
            {
                _provider.OnCommitted(Unit.Default);
            }
        });
    }

    private partial bool HandleApiMessageCore(ApiMessage message);

    private NameValueCollection ParseRequest(ReadOnlyMemory<byte> buffer) =>
        HttpUtility.ParseQueryString(Encoding.UTF8.GetString(buffer.Span));

    private bool ValidateResultCode(int code)
    {
        if (code is 1)
            return true;

        _logger.LogWarning("API call was rejected with result code {ResultCode}", code);
        return false;
    }


    Task IServiceInitializable.InitializeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
