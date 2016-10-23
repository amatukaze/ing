using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class ConstructionRecord : ModelBase
    {
        public string Time { get; }

        public ShipInfo Ship { get; }

        public int FuelConsumption { get; }
        public int BulletConsumption { get; }
        public int SteelConsumption { get; }
        public int BauxiteConsumption { get; }
        public int DevelopmentMaterialConsumption { get; }

        public ShipInfo SecretaryShip { get; }
        public int HeadquarterLevel { get; }

        public bool IsLargeShipConstruction => FuelConsumption >= 1000 && BulletConsumption >= 1000 && SteelConsumption >= 1000 & BauxiteConsumption >= 1000;
        public int? EmptyDockCount { get; }

        internal ConstructionRecord(SQLiteDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(rpReader.GetInt64("time")).LocalDateTime.ToString();
            Ship = KanColleGame.Current.MasterInfo.Ships[rpReader.GetInt32("ship")];

            FuelConsumption = rpReader.GetInt32("fuel");
            BulletConsumption = rpReader.GetInt32("bullet");
            SteelConsumption = rpReader.GetInt32("steel");
            BauxiteConsumption = rpReader.GetInt32("bauxite");
            DevelopmentMaterialConsumption = rpReader.GetInt32("dev_material");

            SecretaryShip = KanColleGame.Current.MasterInfo.Ships[rpReader.GetInt32("flagship")];
            HeadquarterLevel = rpReader.GetInt32("hq_level");

            EmptyDockCount = rpReader.GetInt32Optional("empty_dock");
        }
    }
}
