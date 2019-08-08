using System.Collections.Generic;
using System.Collections.Specialized;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public event TimedMessageHandler<HomeportUpdate> HomeportReturned;
        public event TimedMessageHandler<RawAdmiral> AdmiralUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawFleet>> FleetsUpdated;
        public event TimedMessageHandler<CompositionChange> CompositionChanged;
        public event TimedMessageHandler<RawFleet> FleetPresetSelected;
        public event TimedMessageHandler<ShipId> ShipExtraSlotOpened;
        //public event TimedMessageHandler<ShipEquipmentUpdate> ShipEquipmentUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawShip>> PartialShipsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawFleet>> PartialFleetsUpdated;
        public event TimedMessageHandler<IMaterialsUpdate> MaterialsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawRepairingDock>> RepairingDockUpdated;
        public event TimedMessageHandler<RepairStart> RepairStarted;
        public event TimedMessageHandler<RepairingDockId> InstantRepaired;
        public event TimedMessageHandler<IReadOnlyCollection<ShipSupply>> ShipSupplied;
        public event TimedMessageHandler<ExpeditionCompletion> ExpeditionCompleted;
        public event TimedMessageHandler<IReadOnlyCollection<IRawIncentiveReward>> IncentiveRewarded;

        private static HomeportUpdate ParseHomeport(HomeportJson response)
            => new HomeportUpdate
            (
                response.api_ship,
                response.CombinedFleet
            );

        private static RepairStart ParseRepairStart(NameValueCollection request)
            => new RepairStart
            (
                instantRepair: request.GetBool("api_highspeed"),
                shipId: (ShipId)request.GetInt("api_ship_id"),
                repairingDockId: request.GetInt("api_ndock_id")
            );

        private static RepairingDockId ParseInstantRepair(NameValueCollection request)
            => (RepairingDockId)request.GetInt("api_ndock_id");

        private static CompositionChange ParseCompositionChange(NameValueCollection request)
        {
            var fleetId = (FleetId)request.GetInt("api_id");
            int position = request.GetInt("api_ship_idx");
            var shipId = (ShipId)request.GetInt("api_ship_id");
            if (position == -1 || shipId == -2)
                return new CompositionChange(fleetId, null, null);
            else if (shipId == -1)
                return new CompositionChange(fleetId, position, null);
            else
                return new CompositionChange(fleetId, position, shipId);
        }

        private static ShipId ParseShipExtraSlotOpen(NameValueCollection request)
            => (ShipId)request.GetInt("api_id");

        private static ShipEquipmentUpdate ParseShipEquipmentUpdate(NameValueCollection request, ShipEquipmentJson response)
            => new ShipEquipmentUpdate
            (
                shipId: (ShipId)request.GetInt("api_id"),
                equipmentIds: response.api_slot
            );

        private static RawShip[] ParseShipDeprive(DepriveJson response)
            => new[] { response.api_ship_data.api_unset_ship, response.api_ship_data.api_set_ship };

        private static ExpeditionCompletion ParseExpeditionCompletion(NameValueCollection request, ExpeditionCompletionJson response)
        {
            static UseItemRecord? GetRecord(int flag, ExpeditionCompletionJson.GetItem json)
                => flag switch
                {
                    4 => new UseItemRecord
                    {
                        ItemId = json?.api_useitem_id ?? default,
                        Count = json?.api_useitem_count ?? 0
                    },
                    var x when x > 0 => new UseItemRecord
                    {
                        ItemId = (UseItemId)x,
                        Count = json?.api_useitem_count ?? 0
                    },
                    _ => (UseItemRecord?)null
                };
            return new ExpeditionCompletion
            (
                fleetId: (FleetId)request.GetInt("api_deck_id"),
                expeditionName: response.api_quest_name,
                result: response.api_clear_result,
                materialsAcquired: response.api_get_material,
                rewardItem1: GetRecord(response.api_useitem_flag.At(0), response.api_get_item1),
                rewardItem2: GetRecord(response.api_useitem_flag.At(1), response.api_get_item2)
            );
        }
    }
}
