using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_get_member/mapinfo")]
    private void HandleMaps(MapJson response)
    {
        _provider.OnAirForceGroupsUpdated(response.api_air_base);
    }
}
