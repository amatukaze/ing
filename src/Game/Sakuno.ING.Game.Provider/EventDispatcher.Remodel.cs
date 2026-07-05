using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;
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

    [Api("api_req_kaisou/lock")]
    private void HandleSlotItemLockingUpdated([FromRequest("slotitem_id")] SlotItemId shipId, LockingJson response) =>
        _provider.OnSlotItemPatched(new SlotItemIsLockedPatch(shipId, response.api_locked));

    [Api("api_req_kaisou/marriage")]
    private void HandleShipMarried(RawShip response) =>
        _provider.OnPartialShipsUpdated([response]);
}
