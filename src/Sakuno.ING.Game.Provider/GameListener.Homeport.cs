using System.Collections.Generic;
using System.Collections.Specialized;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<IHomeportUpdate>> HomeportUpdated;
        public readonly IProducer<ITimedMessage<IRawAdmiral>> AdmiralUpdated;
        public readonly IProducer<ITimedMessage<IMaterialsUpdate>> MaterialsUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawRepairingDock>>> RepairingDockUpdated;
        public readonly IProducer<ITimedMessage<RepairStart>> RepairStarted;
        public readonly IProducer<ITimedMessage<RepairingDockId>> InstantRepaired;

        private static RepairStart ParseRepairStart(NameValueCollection request)
            => new RepairStart
            {
                InstantRepair = request.GetBool("api_highspeed"),
                ShipId = request.GetInt("api_ship_id"),
                RepairingDockId = request.GetInt("api_ndock_id")
            };

        private static RepairingDockId ParseInstantRepair(NameValueCollection request)
            => new RepairingDockId(request.GetInt("api_ndock_id"));
    }
}
