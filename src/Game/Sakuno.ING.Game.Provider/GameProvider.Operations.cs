﻿using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        private readonly Subject<FleetCompositionChange> _fleetCompositionChanged = new();

        [Api("api_req_hensei/change")]
        private void HandleFleetCompositionChanged(NameValueCollection request)
        {
            var fleetId = (FleetId)request.GetInt("api_id");
            var position = request.GetInt("api_ship_idx");
            var shipId = (ShipId)request.GetInt("api_ship_id");

            if (position == -1 || shipId == -2)
                _fleetCompositionChanged.OnNext(new FleetCompositionChange(fleetId, null, null));
            else if (shipId == -1)
                _fleetCompositionChanged.OnNext(new FleetCompositionChange(fleetId, position, null));
            else
                _fleetCompositionChanged.OnNext(new FleetCompositionChange(fleetId, position, shipId));
        }

        private readonly Subject<(ShipId, bool)> _shipLockUpdated = new();

        [Api("api_req_hensei/lock")]
        private void HandleShipLockApi(NameValueCollection request, RawShipLockInfo response) =>
            _shipLockUpdated.OnNext(((ShipId)request.GetInt("api_ship_id"), response.api_locked));

        private readonly Subject<ShipSupply[]> _shipsSupplied = new();

        [Api("api_req_hokyu/charge")]
        private void HandleShipsSupplied(ShipsSupplyJson response)
        {
            _shipsSupplied.OnNext(response.api_ship);
            _materialUpdated.OnNext(response);
        }

        private readonly Subject<(SlotItemId, bool)> _slotItemLockUpdated = new();

        [Api("api_req_kaisou/lock")]
        private void HandleSlotItemLockApi(NameValueCollection request, RawSlotItemLockInfo response) =>
            _slotItemLockUpdated.OnNext(((SlotItemId)request.GetInt("api_slotitem_id"), response.api_locked));

        [Api("api_req_kaisou/powerup")]
        private void HandleShipModernization(NameValueCollection request, ShipModernizationResultJson response)
        {
            _partialShipsUpdated.OnNext(new[] { response.api_ship });
            _fleetsUpdated.OnNext(response.api_deck);

            var consumedShipIds = request.GetShipIds("api_id_items");
            if (!request.GetBool("api_slot_dest_flag"))
                _shipsRemoved.OnNext(consumedShipIds);
            else
                _shipsAndSlotItemsRemoved.OnNext(consumedShipIds);
        }

        [Api("api_req_kaisou/marriage")]
        private void HandleShipMarriage(RawShip response) =>
            _partialShipsUpdated.OnNext(new[] { response });

        [Api("api_req_kaisou/slot_deprive")]
        private void HandleSlotItemTransferApi(SlotItemTransferJson response)
        {
            _partialShipsUpdated.OnNext(new[] { response.api_ship_data.api_set_ship, response.api_ship_data.api_unset_ship });
            _materialUpdated.OnNext(response);
        }

        private readonly Subject<RepairStart> _repairStarted = new();
        private readonly Subject<RepairDockId> _instantRepairUsed = new();

        [Api("api_req_nyukyo/start")]
        private void HandleRepairStart(NameValueCollection request) =>
            _repairStarted.OnNext(new
            (
                instantRepair: request.GetBool("api_highspeed"),
                shipId: (ShipId)request.GetInt("api_ship_id"),
                dockId: (RepairDockId)request.GetInt("api_ndock_id")
            ));
        [Api("api_req_nyukyo/speedchange")]
        private void HandleInstantRepair(NameValueCollection request) =>
            _instantRepairUsed.OnNext((RepairDockId)request.GetInt("api_ndock_id"));

        private readonly Subject<ConstructionStart> _constructionStarted = new();
        private readonly Subject<ConstructionDockId> _instantConstructionUsed = new();

        [Api("api_req_kousyou/createship")]
        private void HandleConstructionStart(NameValueCollection request)
        {
            var data = new ConstructionStart
            (
                dockId: (ConstructionDockId)request.GetInt("api_kdock_id"),
                instantBuild: request.GetBool("api_highspeed"),
                isLSC: request.GetBool("api_large_flag"),
                consumption: new Materials()
                {
                    Fuel = request.GetInt("api_item1"),
                    Bullet = request.GetInt("api_item2"),
                    Steel = request.GetInt("api_item3"),
                    Bauxite = request.GetInt("api_item4"),
                    Development = request.GetInt("api_item5"),
                }
            );
            _constructionStarted.OnNext(data);
            _materialUpdated.OnNext(data);
        }
        [Api("api_req_kousyou/createship_speedchange")]
        private void HandleInstantConstruction(NameValueCollection request) =>
            _instantConstructionUsed.OnNext((ConstructionDockId)request.GetInt("api_kdock_id"));

        [Api("api_req_kousyou/getship")]
        private void HandleGetShipApi(ShipConstructionResultJson response)
        {
            _constructionDocksUpdated.OnNext(response.api_kdock);
            _partialShipsUpdated.OnNext(new[] { response.api_ship });
            _partialSlotItemsUpdated.OnNext(response.api_slotitem);
        }

        [Api("api_req_kousyou/createitem")]
        private void HandleSlotItemsDeveloped(SlotItemsDevelopedJson response)
        {
            if (response.api_create_flag)
            {
                var slotItems = new List<RawSlotItem>(3);

                foreach (var item in response.api_get_items)
                    if (item is { api_id: > 0, api_slotitem_id: > 0 })
                        slotItems.Add(new() { Id = (SlotItemId)item.api_id, SlotItemInfoId = (SlotItemInfoId)item.api_slotitem_id });

                _partialSlotItemsUpdated.OnNext(slotItems.ToArray());
            }

            _materialUpdated.OnNext(response);
        }

        [Api("api_req_kousyou/destroyship")]
        private void HandleShipDismantled(NameValueCollection request, ShipsDismantlingJson response)
        {
            var shipIds = request.GetShipIds("api_ship_id");
            if (!request.GetBool("api_slot_dest_flag"))
                _shipsRemoved.OnNext(shipIds);
            else
                _shipsAndSlotItemsRemoved.OnNext(shipIds);

            _materialUpdated.OnNext(response);
        }

        [Api("api_req_kousyou/destroyitem2")]
        private void HandleSlotItemsScrapped(NameValueCollection request, SlotItemsScrappingJson response)
        {
            _slotItemsRemoved.OnNext(request.GetSlotItemIds("api_slotitem_ids"));
            _materialUpdated.OnNext(response);
        }

        [Api("api_req_kousyou/remodel_slot")]
        private void HandleSlotItemImproved(NameValueCollection request, SlotItemImprovementJson response)
        {
            _materialUpdated.OnNext(response);

            if (response.api_remodel_flag)
                _partialSlotItemsUpdated.OnNext(new[] { response.api_after_slot });

            _slotItemsRemoved.OnNext(response.api_use_slot_id);
        }

        private readonly Subject<AirForceActionUpdate[]> _airForceGroupActionUpdated = new();
        private readonly Subject<AirForceSquadronDeployment> _airForceSquadronDeployed = new();
        private readonly Subject<AirForceSquadronSupplied> _airForceSquadronSupplied = new();

        [Api("api_req_air_corps/set_action")]
        private void HandleAirForceGroupActionUpdated(NameValueCollection request)
        {
            var mapArea = request.GetInt("api_area_id");

            _airForceGroupActionUpdated.OnNext(request.GetInts("api_base_id").Zip(request.GetInts("api_action_kind"),
                (id, action) => new AirForceActionUpdate
                (
                    mapAreaId: (MapAreaId)mapArea,
                    groupId: (AirForceGroupId)id,
                    action: (AirForceAction)action
                )).ToArray());
        }

        [Api("api_req_air_corps/set_plane")]
        private void HandleAirForceSquadronDeployment(NameValueCollection request, AirForceSquadronDeploymentJson response)
        {
            _airForceSquadronDeployed.OnNext(new
            (
                mapAreaId: (MapAreaId)request.GetInt("api_area_id"),
                groupId: (AirForceGroupId)request.GetInt("api_base_id"),
                baseCombatRadius: response.api_distance.api_base,
                bonusCombatRadius: response.api_distance.api_bonus,
                updatedSquadrons: response.api_plane_info
            ));
            _materialUpdated.OnNext(response);
        }

        [Api("api_req_air_corps/supply")]
        private void HandleAirForceSquadronSupplied(NameValueCollection request, AirForceSquadronSupplyJson response)
        {
            var mapAreaId = (MapAreaId)request.GetInt("api_area_id");
            var groupId = (AirForceGroupId)request.GetInt("api_base_id");

            _airForceSquadronSupplied.OnNext(new((mapAreaId, groupId), response.api_plane_info));
            _materialUpdated.OnNext(response);
        }
    }
}
