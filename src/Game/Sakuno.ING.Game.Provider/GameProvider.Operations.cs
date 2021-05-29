using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<FleetCompositionChange> FleetCompositionChanged { get; private set; }

        private static FleetCompositionChange ParseFleetCompositionChange(NameValueCollection request)
        {
            var fleetId = (FleetId)request.GetInt("api_id");
            var position = request.GetInt("api_ship_idx");
            var shipId = (ShipId)request.GetInt("api_ship_id");

            if (position == -1 || shipId == -2)
                return new FleetCompositionChange(fleetId, null, null);
            else if (shipId == -1)
                return new FleetCompositionChange(fleetId, position, null);
            else
                return new FleetCompositionChange(fleetId, position, shipId);
        }

        public IObservable<ShipSupply> ShipSupplied { get; private set; }

        public IObservable<ShipModernization> ShipModernization { get; private set; }

        private static ShipModernization ParseShipModernization(NameValueCollection request, ShipModernizationResultJson response) => new
        (
            shipId: (ShipId)request.GetInt("api_id"),
            consumedShipIds: request.GetShipIds("api_id_items"),
            isSuccess: response.api_powerup_flag,
            newRawData: response.api_ship,
            removeSlotItems: request.GetBool("api_slot_dest_flag")
        );

        public IObservable<RepairStart> RepairStarted { get; private set; }
        public IObservable<RepairDockId> InstantRepairUsed { get; private set; }

        private static RepairStart ParseRepairStart(NameValueCollection request) => new
        (
            instantRepair: request.GetBool("api_highspeed"),
            shipId: (ShipId)request.GetInt("api_ship_id"),
            dockId: (RepairDockId)request.GetInt("api_ndock_id")
        );
        private static RepairDockId ParseInstantRepair(NameValueCollection request) =>
            (RepairDockId)request.GetInt("api_ndock_id");

        public IObservable<ConstructionStart> ConstructionStarted { get; private set; }
        public IObservable<ConstructionDockId> InstantConstructionUsed { get; private set; }

        private static ConstructionStart ParseConstructionStart(NameValueCollection request) => new
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
        private static ConstructionDockId ParseInstantConstruction(NameValueCollection request) =>
            (ConstructionDockId)request.GetInt("api_kdock_id");

        public IObservable<SlotItemsDeveloped> SlotItemsDeveloped { get; private set; }

        private static SlotItemsDeveloped ParseSlotItemsDeveloped(SlotItemsDevelopedJson response) => new
        (
            IsSuccessful: response.api_create_flag,
            SlotItems: response.api_get_items.Select(raw => (raw.api_id, raw.api_slotitem_id) switch
            {
                var (id, masterId) when id > 0 && masterId > 0 => new RawSlotItem() { Id = (SlotItemId)id, SlotItemInfoId = (SlotItemInfoId)masterId },
                _ => null,
            }).ToArray()
        );

        public IObservable<ShipsDismantled> ShipsDismantled { get; private set; }
        public IObservable<SlotItemId[]> SlotItemsScrapped { get; private set; }

        private static ShipsDismantled ParseShipDismantled(SvDataWithRequest<ShipsDismantlingJson> rawData) => new
        (
            ShipIds: rawData.Request.GetShipIds("api_ship_id"),
            RemoveSlotItems: rawData.Request.GetBool("api_slot_dest_flag")
        );

        public IObservable<SlotItemImproved> SlotItemImproved { get; private set; }

        private static SlotItemImproved ParseSlotItemImproved(SvDataWithRequest<SlotItemImprovementJson> rawData) => new
        (
            SlotItemId: (SlotItemId)rawData.Request.GetInt("api_slot_id"),
            IsSuccessful: rawData.api_data.api_remodel_flag,
            NewRawData: rawData.api_data.api_after_slot,
            ConsumedSlotItemIds: rawData.api_data.api_use_slot_id
        );

        public IObservable<AirForceActionUpdate> AirForceGroupActionUpdated { get; private set; }
        public IObservable<AirForceSquadronDeployment> AirForceSquadronDeployed { get; private set; }
        public IObservable<AirForceSquadronSupplied> AirForceSquadronSupplied { get; private set; }

        private static IEnumerable<AirForceActionUpdate> ParseAirForceGroupActionUpdates(NameValueCollection request)
        {
            var mapArea = request.GetInt("api_area_id");

            return request.GetInts("api_base_id").Zip(request.GetInts("api_action_kind"),
                (id, action) => new AirForceActionUpdate
                (
                    mapAreaId: (MapAreaId)mapArea,
                    groupId: (AirForceGroupId)id,
                    action: (AirForceAction)action
                ));
        }
        private static AirForceSquadronDeployment ParseAirForceSquadronDeployment(NameValueCollection request, AirForceSquadronDeploymentJson response) => new
        (
            mapAreaId: (MapAreaId)request.GetInt("api_area_id"),
            groupId: (AirForceGroupId)request.GetInt("api_base_id"),
            baseCombatRadius: response.api_distance.api_base,
            bonusCombatRadius: response.api_distance.api_bonus,
            updatedSquadrons: response.api_plane_info
        );
        private static AirForceSquadronSupplied ParseAirForceSquadronSupply(NameValueCollection request, AirForceSquadronSupplyJson response)
        {
            var mapAreaId = (MapAreaId)request.GetInt("api_area_id");
            var groupId = (AirForceGroupId)request.GetInt("api_base_id");

            return new((mapAreaId, groupId), response.api_plane_info);
        }
    }
}
