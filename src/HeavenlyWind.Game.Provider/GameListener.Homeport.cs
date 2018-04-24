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
        public readonly IProducer<ITimedMessage<Materials>> MaterialsUpdated;

        private static HomeportUpdate ParseHomeport(HomeportJson raw)
            => new HomeportUpdate
            {
                Fleets = raw.api_deck_port,
                Ships = raw.api_ship,
                CombinedFleetType = raw.api_combined_flag
            };

        private static Materials ParseMaterials(MaterialJson[] raw)
        {
            Materials result = default;
            foreach (var r in raw)
                switch (r.api_id)
                {
                    case 1:
                        result.Fuel = r.api_value;
                        break;
                    case 2:
                        result.Bullet = r.api_value;
                        break;
                    case 3:
                        result.Steel = r.api_value;
                        break;
                    case 4:
                        result.Bauxite = r.api_value;
                        break;
                    case 5:
                        result.InstantBuild = r.api_value;
                        break;
                    case 6:
                        result.InstantRepair = r.api_value;
                        break;
                    case 7:
                        result.Development = r.api_value;
                        break;
                    case 8:
                        result.Improvement = r.api_value;
                        break;
                }
            return result;
        }
    }
}
