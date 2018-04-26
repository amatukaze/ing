using System.Collections.Generic;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<HomeportUpdate>> HomeportUpdated;
        public readonly IProducer<ITimedMessage<IRawAdmiral>> AdmiralUpdated;
        public readonly IProducer<ITimedMessage<MaterialUpdates>> MaterialsUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawRepairingDock>>> RepairingDockUpdated;

        private static HomeportUpdate ParseHomeport(HomeportJson raw)
            => new HomeportUpdate
            {
                Fleets = raw.api_deck_port,
                Ships = raw.api_ship,
                CombinedFleetType = raw.api_combined_flag
            };
    }
}
