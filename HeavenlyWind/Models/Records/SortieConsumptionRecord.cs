using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Data.SQLite;
using EventMapDifficultyEnum = Sakuno.KanColle.Amatsukaze.Game.Models.EventMapDifficulty;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class SortieConsumptionRecord : ModelBase
    {
        public string Time { get; }

        public IMapMasterInfo Map { get; }
        public bool IsEventMap { get; }
        public EventMapDifficultyEnum? EventMapDifficulty { get; }

        public int Fuel { get; }
        public int Bullet { get; }
        public int Steel { get; }
        public int Bauxite { get; }
        public int Bucket { get; }

        public double RankingPoint { get; }

        internal SortieConsumptionRecord(SQLiteDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(Convert.ToInt64(rpReader["id"])).LocalDateTime.ToString();

            var rMapID = Convert.ToInt32(rpReader["map"]);
            Map = MapService.Instance.GetMasterInfo(rMapID);

            var rEventMapDifficulty = rpReader["difficulty"];
            if (rEventMapDifficulty != DBNull.Value)
            {
                IsEventMap = true;
                EventMapDifficulty = (EventMapDifficultyEnum)Convert.ToInt32(rEventMapDifficulty);
            }

            Fuel = Convert.ToInt32(rpReader["fuel"]);
            Bullet = Convert.ToInt32(rpReader["bullet"]);
            Steel = Convert.ToInt32(rpReader["steel"]);
            Bauxite = Convert.ToInt32(rpReader["bauxite"]);
            Bucket = Convert.ToInt32(rpReader["bucket"]);

            RankingPoint = (double)rpReader["ranking_point"];
        }
    }
}
