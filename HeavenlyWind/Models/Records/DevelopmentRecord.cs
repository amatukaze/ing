using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class DevelopmentRecord : ModelBase
    {
        public string Time { get; }

        public EquipmentInfo Equipment { get; }

        public int FuelConsumption { get; }
        public int BulletConsumption { get; }
        public int SteelConsumption { get; }
        public int BauxiteConsumption { get; }

        public ShipInfo SecretaryShip { get; }
        public int HeadquarterLevel { get; }

        internal DevelopmentRecord(SQLiteDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(rpReader.GetInt64("time")).LocalDateTime.ToString();
            var rEquipmentID = rpReader.GetInt32Optional("equipment");
            if (rEquipmentID.HasValue)
                Equipment = KanColleGame.Current.MasterInfo.Equipment[rEquipmentID.Value];

            FuelConsumption = rpReader.GetInt32("fuel");
            BulletConsumption = rpReader.GetInt32("bullet");
            SteelConsumption = rpReader.GetInt32("steel");
            BauxiteConsumption = rpReader.GetInt32("bauxite");

            SecretaryShip = KanColleGame.Current.MasterInfo.Ships[rpReader.GetInt32("flagship")];
            HeadquarterLevel = rpReader.GetInt32("hq_level");
        }
    }
}
