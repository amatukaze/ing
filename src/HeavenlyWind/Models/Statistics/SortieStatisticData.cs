using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Statistics
{
    class SortieStatisticData : SortieRecordBase
    {
        public int Count { get; }
        public int BossBattleCount { get; }

        public int FuelConsumption { get; }
        public int BulletConsumption { get; }
        public int SteelConsumption { get; }
        public int BauxiteConsumption { get; }
        public int BucketConsumption { get; }

        public double RankingPoint { get; }

        public int SRankCount { get; }
        public int ARankCount { get; }
        public int BRankCount { get; }
        public int FailureRankCount { get; }

        public int BossSRankCount { get; }
        public int BossARankCount { get; }
        public int BossBRankCount { get; }
        public int BossFailureRankCount { get; }

        public SortieStatisticData(SQLiteDataReader rpReader) : base(rpReader)
        {
            Count = rpReader.GetInt32("count");
            BossBattleCount = rpReader.GetInt32("battle_boss_count");

            FuelConsumption = rpReader.GetInt32("fuel");
            BulletConsumption = rpReader.GetInt32("bullet");
            SteelConsumption = rpReader.GetInt32("steel");
            BauxiteConsumption = rpReader.GetInt32("bauxite");
            BucketConsumption = rpReader.GetInt32("bucket");

            RankingPoint = rpReader.GetDouble("ranking_point");

            SRankCount = rpReader.GetInt32("S");
            ARankCount = rpReader.GetInt32("A");
            BRankCount = rpReader.GetInt32("B");
            FailureRankCount = rpReader.GetInt32("F");

            BossSRankCount = rpReader.GetInt32("S_boss");
            BossARankCount = rpReader.GetInt32("A_boss");
            BossBRankCount = rpReader.GetInt32("B_boss");
            BossFailureRankCount = rpReader.GetInt32("F_boss");
        }
    }
}
