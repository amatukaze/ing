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
        public readonly IProducer<ITimedMessage<IHomeportUpdate>> HomeportReturned;
        public readonly IProducer<ITimedMessage<IRawAdmiral>> AdmiralUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawFleet>>> FleetsUpdated;
        public readonly IProducer<ITimedMessage<CompositionChange>> CompositionChanged;
        public readonly IProducer<ITimedMessage<IRawFleet>> FleetPresetSelected;
        public readonly IProducer<ITimedMessage<ShipId>> ShipExtraSlotOpened;
        public readonly IProducer<ITimedMessage<ShipEquipmentUpdate>> ShipEquipmentUdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawShip>>> PartialShipsUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawFleet>>> PartialFleetsUpdated;
        public readonly IProducer<ITimedMessage<IMaterialsUpdate>> MaterialsUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawRepairingDock>>> RepairingDockUpdated;
        public readonly IProducer<ITimedMessage<RepairStart>> RepairStarted;
        public readonly IProducer<ITimedMessage<RepairingDockId>> InstantRepaired;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IShipSupply>>> ShipSupplied;

        private static RepairStart ParseRepairStart(NameValueCollection request)
            => new RepairStart
            {
                InstantRepair = request.GetBool("api_highspeed"),
                ShipId = request.GetInt("api_ship_id"),
                RepairingDockId = request.GetInt("api_ndock_id")
            };

        private static RepairingDockId ParseInstantRepair(NameValueCollection request)
            => new RepairingDockId(request.GetInt("api_ndock_id"));

        private static CompositionChange ParseCompositionChange(NameValueCollection request)
        {
            int fleetId = request.GetInt("api_id");
            int position = request.GetInt("api_ship_idx");
            int shipId = request.GetInt("api_ship_id");
            if (position == -1)
                return new CompositionChange
                {
                    FleetId = fleetId,
                    Index = null,
                    ShipId = null
                };
            else if (shipId == -1)
                return new CompositionChange
                {
                    FleetId = fleetId,
                    Index = position - 1,
                    ShipId = shipId
                };
            else
                return new CompositionChange
                {
                    FleetId = fleetId,
                    Index = position - 1,
                    ShipId = null
                };
        }

        private static ShipId ParseShipExtraSlotOpen(NameValueCollection request)
            => new ShipId(request.GetInt("api_id"));

        private static ShipEquipmentUpdate ParseShipEquipmentUpdate(NameValueCollection request, ShipEquipmentJson response)
            => new ShipEquipmentUpdate
            {
                ShipId = request.GetInt("api_id"),
                EquipmentIds = response.api_slot
            };

        private static ShipJson[] ParseShipDeprive(DepriveJson response)
            => new[] { response.api_ship_data.api_set_ship, response.api_ship_data.api_unset_ship };
    }
}
