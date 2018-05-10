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
        public readonly ITimedMessageProvider<IHomeportUpdate> HomeportReturned;
        public readonly ITimedMessageProvider<IRawAdmiral> AdmiralUpdated;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawFleet>> FleetsUpdated;
        public readonly ITimedMessageProvider<CompositionChange> CompositionChanged;
        public readonly ITimedMessageProvider<IRawFleet> FleetPresetSelected;
        public readonly ITimedMessageProvider<ShipId> ShipExtraSlotOpened;
        public readonly ITimedMessageProvider<ShipEquipmentUpdate> ShipEquipmentUdated;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawShip>> PartialShipsUpdated;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawFleet>> PartialFleetsUpdated;
        public readonly ITimedMessageProvider<IMaterialsUpdate> MaterialsUpdated;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawRepairingDock>> RepairingDockUpdated;
        public readonly ITimedMessageProvider<RepairStart> RepairStarted;
        public readonly ITimedMessageProvider<RepairingDockId> InstantRepaired;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IShipSupply>> ShipSupplied;

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
