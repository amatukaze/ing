using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;
using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_req_kousyou/createship_speedchange")]
    internal void HandleInstantBuildUsed([FromRequest("kdock_id")] ConstructionDockId dockId) =>
        _provider.OnConstructionDockPatched(new ConstructionDockCompletedPatch(dockId, ConstructionDockState.Completed, DateTimeOffset.MinValue));

    [Api("api_req_kousyou/getship")]
    internal void HandleShipBuilt(GetShipJson response)
    {
        _provider.OnConstructionDocksUpdated(response.api_kdock);
        _provider.OnPartialShipsUpdated([response.api_ship]);
        _provider.OnPartialSlotItemsUpdated(response.api_slotitem);
    }
}
