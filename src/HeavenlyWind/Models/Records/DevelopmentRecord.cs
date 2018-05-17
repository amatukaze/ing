using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class DevelopmentRecord : ModelBase, IRecordID
    {
        public long ID { get; set; }
        public string Time => DateTimeUtil.FromUnixTime(ID).LocalDateTime.ToString();

        public EquipmentInfo Equipment { get; }

        public int FuelConsumption { get; }
        public int BulletConsumption { get; }
        public int SteelConsumption { get; }
        public int BauxiteConsumption { get; }

        public ShipInfo SecretaryShip { get; }
        public int HeadquarterLevel { get; }

        internal DevelopmentRecord(SQLiteDataReader rpReader)
        {
            ID = rpReader.GetInt64("time");

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
