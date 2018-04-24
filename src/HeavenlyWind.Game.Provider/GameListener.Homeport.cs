using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Messaging;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<HomeportUpdate>> HomeportUpdated;
        public readonly IProducer<ITimedMessage<IRawAdmiral>> AdmiralUpdated;
        public readonly IProducer<ITimedMessage<MaterialUpdates>> MaterialsUpdated;

        private static HomeportUpdate ParseHomeport(HomeportJson raw)
            => new HomeportUpdate
            {
                Fleets = raw.api_deck_port,
                Ships = raw.api_ship,
                CombinedFleetType = raw.api_combined_flag
            };
    }
}
