using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public abstract class MapInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }

        public MapAreaInfo Area { get; protected set; }

        public IReadOnlyCollection<FleetType> AvailableFleetTypes { get; protected set; }

        public int MapBgmId { get; protected set; }

        public int NormalBattleDayBgmId { get; protected set; }
        public int NormalBattleNightBgmId { get; protected set; }

        public int BossBattleDayBgmId { get; protected set; }
        public int BossBattleNightBgmId { get; protected set; }
    }
}
