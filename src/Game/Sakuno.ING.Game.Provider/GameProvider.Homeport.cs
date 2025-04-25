using Sakuno.ING.Game.Provider.Event;
using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

public sealed partial class GameProvider
{
    [Api("api_get_member/require_info")]
    private void HandleStartupInfo(PreHomeportJson response)
    {
        _admiralIdUpdated.OnNext(response.api_basic.api_member_id);
        _slotItemsUpdated.OnNext(response.api_slot_item);
        _constructionDocksUpdated.OnNext(response.api_kdock);
        _useItemsUpdated.OnNext(response.api_useitem);
        _unequippedSlotItemsUpdated.OnNext(response.api_unsetslot);
    }

    [Api("api_get_member/basic")]
    private void HandleAdmiral(RawAdmiral response) =>
        _admiralUpdated.OnNext(response);

    [Api("api_port/port")]
    private void HandleHomeport(HomeportJson response)
    {
        _admiralUpdated.OnNext(response.api_basic);
        _materialsUpdated.OnNext(new HomeportMaterialsUpdate(response.api_material));
        _shipsUpdated.OnNext(response.api_ship);
        _fleetsUpdated.OnNext(response.api_deck_port);
        _repairDocksUpdated.OnNext(response.api_ndock);
    }

    [Api("api_get_member/material")]
    private void HandleMaterials(RawMaterial[] response) =>
        _materialsUpdated.OnNext(new HomeportMaterialsUpdate(response));

    [Api("api_get_member/deck")]
    private void HandleFleetsUpdated(RawFleet[] response) =>
        _fleetsUpdated.OnNext(response);

    [Api("api_get_member/ndock")]
    private void HandleRepairDocksUpdated(RawRepairDock[] response) =>
        _repairDocksUpdated.OnNext(response);

    [Api("api_get_member/slot_item")]
    private void HandleSlotItemsUpdated(RawSlotItem[] response) =>
        _slotItemsUpdated.OnNext(response);

    [Api("api_get_member/kdock")]
    private void HandleConstructionDocksUpdated(RawConstructionDock[] response) =>
        _constructionDocksUpdated.OnNext(response);

    [Api("api_get_member/useitem")]
    private void HandleUseItemsUpdated(RawUseItemCount[] response) =>
        _useItemsUpdated.OnNext(response);

    [Api("api_get_member/unsetslot")]
    private void HandleUnequippedSlotItemsUpdated(RawUnequippedSlotItems[] response) =>
        _unequippedSlotItemsUpdated.OnNext(response);
}
