using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using System.Reactive.Subjects;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        private readonly Subject<RawAdmiral> _admiralUpdated = new();

        private readonly Subject<RawSlotItem[]> _slotItemsUpdated = new();
        private readonly Subject<RawConstructionDock[]> _constructionDocksUpdated = new();
        private readonly Subject<RawUseItemCount[]> _useItemsUpdated = new();
        private readonly Subject<RawUnequippedSlotItemInfo[]> _unequippedSlotItemInfoUpdated = new();

        private readonly Subject<RawShip[]> _shipsUpdated = new();
        private readonly Subject<RawRepairDock[]> _repairDocksUpdated = new();
        private readonly Subject<RawFleet[]> _fleetsUpdated = new();

        private readonly Subject<RawShip[]> _partialShipsUpdated = new();
        private readonly Subject<RawSlotItem[]> _partialSlotItemsUpdated = new();
        private readonly Subject<RawFleet[]> _partialFleetsUpdated = new();

        private readonly Subject<IMaterialUpdate> _materialUpdated = new();

        [Api("api_get_member/require_info")]
        private void HandleStartupInfo(StartupInfoJson response)
        {
            _slotItemsUpdated.OnNext(response.api_slot_item);
            _constructionDocksUpdated.OnNext(response.api_kdock);
            _useItemsUpdated.OnNext(response.api_useitem);
            _unequippedSlotItemInfoUpdated.OnNext(response.api_unsetslot);
        }

        [Api("api_port/port")]
        private void HandleHomeportData(HomeportJson response)
        {
            _admiralUpdated.OnNext(response.api_basic);
            _materialUpdated.OnNext(new HomeportMaterialUpdate(response.api_material));
            _shipsUpdated.OnNext(response.api_ship);
            _fleetsUpdated.OnNext(response.api_deck_port);
            _repairDocksUpdated.OnNext(response.api_ndock);
        }

        [Api("api_get_member/deck")]
        private void HandleFleetsUpdated(RawFleet[] response) =>
            _fleetsUpdated.OnNext(response);

        [Api("api_get_member/ndock")]
        private void HandleRepairDocksUpdated(RawRepairDock[] response) =>
            _repairDocksUpdated.OnNext(response);

        [Api("api_get_member/slot_item")]
        private void HandleSlotItemsUpdated(RawSlotItem[] response) =>
            _slotItemsUpdated.OnNext(response);

        [Api("api_get_member/useitem")]
        private void HandleSlotItemsUpdated(RawUseItemCount[] response) =>
            _useItemsUpdated.OnNext(response);

        [Api("api_get_member/kdock")]
        private void HandleConstructionDocksUpdated(RawConstructionDock[] response) =>
            _constructionDocksUpdated.OnNext(response);

        [Api("api_get_member/basic")]
        private void HandleBasicAdmiralUpdated(BasicAdmiral response) =>
            _admiralUpdated.OnNext(response);
        [Api("api_get_member/record")]
        private void HandleRecordAdmiralUpdated(RecordAdmiral response) =>
            _admiralUpdated.OnNext(response);

        [Api("api_get_member/material")]
        private void HandleMaterialUpdated(RawMaterialItem[] response) =>
            _materialUpdated.OnNext(new HomeportMaterialUpdate(response));

        [Api("api_get_member/ship2")]
        private void HandleShip2Api(RawShip[] response)
        {
            _partialShipsUpdated.OnNext(response);
        }
        [Api("api_get_member/ship3")]
        private void HandleShip3Api(Ship3Json response)
        {
            _fleetsUpdated.OnNext(response.api_deck_data);
            _partialShipsUpdated.OnNext(response.api_ship_data);
            _unequippedSlotItemInfoUpdated.OnNext(response.api_slot_data);
        }

        [Api("api_get_member/ship_deck")]
        private void HandleShipDeckApi(ShipDeckJson response)
        {
            _partialShipsUpdated.OnNext(response.api_ship_data);
            _partialFleetsUpdated.OnNext(response.api_deck_data);
        }

        [Api("api_get_member/unsetslot")]
        private void HandleUnequippedSlotItemApi(RawUnequippedSlotItemInfo[] response) =>
            _unequippedSlotItemInfoUpdated.OnNext(response);

        [Api("api_req_hensei/preset_select")]
        private void HandleFleetPresetSelected(RawFleet response) =>
            _partialFleetsUpdated.OnNext(new[] { response });
    }
}
