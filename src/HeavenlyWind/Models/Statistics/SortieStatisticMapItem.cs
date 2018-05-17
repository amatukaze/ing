using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Models.Statistics
{
    class SortieStatisticMapItem : ModelBase
    {
        SortieStatisticData r_Data;

        public IMapMasterInfo Map => r_Data.Map;
        public bool IsEventMap => r_Data.IsEventMap;
        public EventMapDifficulty? EventMapDifficulty => r_Data.EventMapDifficulty;

        public int Count => r_Data.Count;
        public int BossBattleCount => r_Data.BossBattleCount;

        public int FuelConsumption => r_Data.FuelConsumption;
        public int BulletConsumption => r_Data.BulletConsumption;
        public int SteelConsumption => r_Data.SteelConsumption;
        public int BauxiteConsumption => r_Data.BauxiteConsumption;
        public int BucketConsumption => r_Data.BucketConsumption;

        public double RankingPoint => r_Data.RankingPoint;

        public int SRankCount => r_Data.SRankCount;
        public int ARankCount => r_Data.ARankCount;
        public int BRankCount => r_Data.BRankCount;
        public int FailureRankCount => r_Data.FailureRankCount;

        public int BossSRankCount => r_Data.BossSRankCount;
        public int BossARankCount => r_Data.BossARankCount;
        public int BossBRankCount => r_Data.BossBRankCount;
        public int BossFailureRankCount => r_Data.BossFailureRankCount;

        public SortieStatisticMapItem(SortieStatisticData rpData)
        {
            r_Data = rpData;
        }
    }
}
