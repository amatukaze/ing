using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_get_member/mapinfo")]
    private void HandleMaps(MapJson response)
    {
        _airForceGroupsUpdated.OnNext(response.api_air_base);
    }
}
