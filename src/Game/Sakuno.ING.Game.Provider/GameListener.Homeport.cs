using System.Collections.Generic;
using System.Collections.Specialized;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        #region Events
        private readonly ITimedMessageProvider<HomeportUpdate> homeportReturned;
        public event TimedMessageHandler<HomeportUpdate> HomeportReturned
        {
            add => homeportReturned.Received += value;
            remove => homeportReturned.Received -= value;
        }

        private readonly ITimedMessageProvider<IRawAdmiral> admiralUpdated;
        public event TimedMessageHandler<IRawAdmiral> AdmiralUpdated
        {
            add => admiralUpdated.Received += value;
            remove => admiralUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawFleet>> fleetsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawFleet>> FleetsUpdated
        {
            add => fleetsUpdated.Received += value;
            remove => fleetsUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<CompositionChange> compositionChanged;
        public event TimedMessageHandler<CompositionChange> CompositionChanged
        {
            add => compositionChanged.Received += value;
            remove => compositionChanged.Received -= value;
        }

        private readonly ITimedMessageProvider<IRawFleet> fleetPresetSelected;
        public event TimedMessageHandler<IRawFleet> FleetPresetSelected
        {
            add => fleetPresetSelected.Received += value;
            remove => fleetPresetSelected.Received -= value;
        }

        private readonly ITimedMessageProvider<ShipId> shipExtraSlotOpened;
        public event TimedMessageHandler<ShipId> ShipExtraSlotOpened
        {
            add => shipExtraSlotOpened.Received += value;
            remove => shipExtraSlotOpened.Received -= value;
        }

        private readonly ITimedMessageProvider<ShipEquipmentUpdate> shipEquipmentUpdated;
        public event TimedMessageHandler<ShipEquipmentUpdate> ShipEquipmentUpdated
        {
            add => shipEquipmentUpdated.Received += value;
            remove => shipEquipmentUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawShip>> partialShipsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawShip>> PartialShipsUpdated
        {
            add => partialShipsUpdated.Received += value;
            remove => partialShipsUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawFleet>> partialFleetsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawFleet>> PartialFleetsUpdated
        {
            add => partialFleetsUpdated.Received += value;
            remove => partialFleetsUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IMaterialsUpdate> materialsUpdated;
        public event TimedMessageHandler<IMaterialsUpdate> MaterialsUpdated
        {
            add => materialsUpdated.Received += value;
            remove => materialsUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawRepairingDock>> repairingDockUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawRepairingDock>> RepairingDockUpdated
        {
            add => repairingDockUpdated.Received += value;
            remove => repairingDockUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<RepairStart> repairStarted;
        public event TimedMessageHandler<RepairStart> RepairStarted
        {
            add => repairStarted.Received += value;
            remove => repairStarted.Received -= value;
        }

        private readonly ITimedMessageProvider<RepairingDockId> instantRepaired;
        public event TimedMessageHandler<RepairingDockId> InstantRepaired
        {
            add => instantRepaired.Received += value;
            remove => instantRepaired.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IShipSupply>> shipSupplied;
        public event TimedMessageHandler<IReadOnlyCollection<IShipSupply>> ShipSupplied
        {
            add => shipSupplied.Received += value;
            remove => shipSupplied.Received -= value;
        }

        private readonly ITimedMessageProvider<ExpeditionCompletion> expeditionCompleted;
        public event TimedMessageHandler<ExpeditionCompletion> ExpeditionCompleted
        {
            add => expeditionCompleted.Received += value;
            remove => expeditionCompleted.Received -= value;
        }
        #endregion

        private static HomeportUpdate ParseHomeport(HomeportJson response)
            => new HomeportUpdate
            (
                response.api_ship,
                response.api_combined_flag
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

        private static ShipJson[] ParseShipDeprive(DepriveJson response)
            => new[] { response.api_ship_data.api_set_ship, response.api_ship_data.api_unset_ship };

        private static ExpeditionCompletion ParseExpeditionCompletion(NameValueCollection request, ExpeditionCompletionJson response)
        {
            ItemRecord? item1 = null, item2 = null;

            var id1 = response.api_useitem_flag.ElementAtOrDefault(0);
            if (id1 == 4)
                item1 = new ItemRecord
                {
                    ItemId = response.api_get_item1.api_useitem_id,
                    Count = response.api_get_item1.api_useitem_count
                };
            else if (id1 > 0)
                item1 = new ItemRecord
                {
                    ItemId = id1,
                    Count = response.api_get_item1.api_useitem_count
                };

            var id2 = response.api_useitem_flag.ElementAtOrDefault(1);
            if (id2 == 4)
                item2 = new ItemRecord
                {
                    ItemId = response.api_get_item2.api_useitem_id,
                    Count = response.api_get_item2.api_useitem_count
                };
            else if (id2 > 0)
                item2 = new ItemRecord
                {
                    ItemId = id2,
                    Count = response.api_get_item2.api_useitem_count
                };

            return new ExpeditionCompletion
            (
                fleetId: (FleetId)request.GetInt("api_deck_id"),
                expeditionName: response.api_quest_name,
                result: response.api_clear_result,
                materialsAcquired: response.api_get_material,
                rewardItem1: item1,
                rewardItem2: item2
            );
        }
    }
}
