using Sakuno.ING.Game.Provider.Event;
using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_get_member/require_info")]
    private void HandleStartupInfo(PreHomeportJson response)
    {
        _provider.OnAdmiralIdUpdated(response.api_basic.api_member_id);
        _provider.OnSlotItemsUpdated(response.api_slot_item);
        _provider.OnConstructionDocksUpdated(response.api_kdock);
        _provider.OnUseItemsUpdated(response.api_useitem);
        _provider.OnUnequippedSlotItemsUpdated(response.api_unsetslot);
    }

    [Api("api_get_member/basic")]
    private void HandleAdmiral(RawAdmiral response) =>
        _provider.OnAdmiralUpdated(response);

    [Api("api_port/port")]
    private void HandleHomeport(HomeportJson response)
    {
        _provider.OnAdmiralUpdated(response.api_basic);
        _provider.OnMaterialsUpdated(new HomeportMaterialsUpdate(response.api_material));
        _provider.OnShipsUpdated(response.api_ship);
        _provider.OnFleetsUpdated(response.api_deck_port);
        _provider.OnRepairDocksUpdated(response.api_ndock);
    }

    [Api("api_get_member/material")]
    private void HandleMaterials(RawMaterial[] response) =>
        _provider.OnMaterialsUpdated(new HomeportMaterialsUpdate(response));

    [Api("api_get_member/deck")]
    private void HandleFleetsUpdated(RawFleet[] response) =>
        _provider.OnFleetsUpdated(response);

    [Api("api_get_member/ndock")]
    private void HandleRepairDocksUpdated(RawRepairDock[] response) =>
        _provider.OnRepairDocksUpdated(response);

    [Api("api_get_member/slot_item")]
    private void HandleSlotItemsUpdated(RawSlotItem[] response) =>
        _provider.OnSlotItemsUpdated(response);

    [Api("api_get_member/kdock")]
    private void HandleConstructionDocksUpdated(RawConstructionDock[] response) =>
        _provider.OnConstructionDocksUpdated(response);

    [Api("api_get_member/useitem")]
    private void HandleUseItemsUpdated(RawUseItemCount[] response) =>
        _provider.OnUseItemsUpdated(response);

    [Api("api_get_member/unsetslot")]
    private void HandleUnequippedSlotItemsUpdated(RawUnequippedSlotItems[] response) =>
        _provider.OnUnequippedSlotItemsUpdated(response);
}
