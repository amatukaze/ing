using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

public sealed partial class GameProvider
{
    [Api("api_start2/getData")]
    private void HandleMasterData(MasterDataJson response)
    {
        _masterDataUpdated.OnNext(response);
    }
}
