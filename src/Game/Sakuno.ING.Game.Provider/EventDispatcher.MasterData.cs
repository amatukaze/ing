using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_start2/getData")]
    private void HandleMasterData(MasterDataJson response)
    {
        _provider.OnMasterDataUpdated(response);
    }
}
