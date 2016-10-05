using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Statistics
{
    class SortieStatisticData : SortieRecordBase
    {
        public SortieStatisticTimeSpanType Type { get; }

        public int Count { get; }

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

        public SortieStatisticData(SQLiteDataReader rpReader) : base(rpReader)
        {
            Type = (SortieStatisticTimeSpanType)rpReader.GetInt32("type");

            Count = rpReader.GetInt32("count");

            FuelConsumption = rpReader.GetInt32("fuel_consumption");
            BulletConsumption = rpReader.GetInt32("bullet_consumption");
            SteelConsumption = rpReader.GetInt32("steel_consumption");
            BauxiteConsumption = rpReader.GetInt32("bauxite_consumption");
            BucketConsumption = rpReader.GetInt32("bucket_consumption");

            RankingPoint = rpReader.GetDouble("ranking_point");

            SRankCount = rpReader.GetInt32("s_rank_count");
            ARankCount = rpReader.GetInt32("a_rank_count");
            BRankCount = rpReader.GetInt32("b_rank_count");
            FailureRankCount = rpReader.GetInt32("failure_rank_count");
        }
    }
}
