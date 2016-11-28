using Sakuno.KanColle.Amatsukaze.Game;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class SortieConsumptionRecord : SortieRecordBase, IRecordID
    {
        public long ID { get; set; }
        public string Time => DateTimeUtil.FromUnixTime(ID).LocalDateTime.ToString();

        public ClampedValue MapHP { get; }

        public int Fuel { get; }
        public int Bullet { get; }
        public int Steel { get; }
        public int Bauxite { get; }
        public int Bucket { get; }

        public double? RankingPoint { get; }

        internal SortieConsumptionRecord(SQLiteDataReader rpReader) : base(rpReader)
        {
            ID = rpReader.GetInt64("time");

            var rMapMaxHP = rpReader.GetInt32Optional("map_max_hp");
            if (rMapMaxHP.HasValue)
            {
                var rMapHP = rpReader.GetInt32Optional("map_hp");
                if (rMapHP.HasValue)
                    MapHP = new ClampedValue(rMapMaxHP.Value, rMapHP.Value);
            }

            Fuel = rpReader.GetInt32("fuel");
            Bullet = rpReader.GetInt32("bullet");
            Steel = rpReader.GetInt32("steel");
            Bauxite = rpReader.GetInt32("bauxite");
            Bucket = rpReader.GetInt32("bucket");

            RankingPoint = rpReader.GetDoubleOptional("ranking_point");
        }
    }
}
