using Sakuno.ING.Game.Patches;
using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_req_hokyu/charge")]
    private void HandleShipSupplied(ShipSuppliedJson response)
    {
        Mutate((masterData, playerData) =>
        {
            foreach (var ship in response.api_ship)
                _provider.OnShipPatched(new ShipSupplyPatch(ship.api_id, ship.api_fuel, ship.api_bull, ship.api_onslot));
        });
    }
}
