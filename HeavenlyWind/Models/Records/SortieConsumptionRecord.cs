using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class SortieConsumptionRecord : SortieRecordBase
    {
        public string Time { get; }

        public int Fuel { get; }
        public int Bullet { get; }
        public int Steel { get; }
        public int Bauxite { get; }
        public int Bucket { get; }

        public double RankingPoint { get; }

        internal SortieConsumptionRecord(SQLiteDataReader rpReader) : base(rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(Convert.ToInt64(rpReader["id"])).LocalDateTime.ToString();

            Fuel = Convert.ToInt32(rpReader["fuel"]);
            Bullet = Convert.ToInt32(rpReader["bullet"]);
            Steel = Convert.ToInt32(rpReader["steel"]);
            Bauxite = Convert.ToInt32(rpReader["bauxite"]);
            Bucket = Convert.ToInt32(rpReader["bucket"]);

            var rRankingPoint = rpReader["ranking_point"];
            if (rRankingPoint != DBNull.Value)
                RankingPoint = Convert.ToDouble(rRankingPoint);
        }
    }
}
