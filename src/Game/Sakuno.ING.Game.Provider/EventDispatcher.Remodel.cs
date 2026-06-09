using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_get_member/ship3")]
    private void HandleShipRemodelled(Ship3Json response)
    {
        _provider.OnPartialShipsUpdated(response.api_ship_data);
        _provider.OnPartialFleetsUpdated(response.api_deck_data);
    }
}
