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
            Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["time"])).LocalDateTime.ToString();
            var rEquipmentID = rpReader["equipment"];
            if (rEquipmentID != DBNull.Value)
                Equipment = KanColleGame.Current.MasterInfo.Equipment[Convert.ToInt32(rEquipmentID)];

            FuelConsumption = Convert.ToInt32(rpReader["fuel"]);
            BulletConsumption = Convert.ToInt32(rpReader["bullet"]);
            SteelConsumption = Convert.ToInt32(rpReader["steel"]);
            BauxiteConsumption = Convert.ToInt32(rpReader["bauxite"]);

            SecretaryShip = KanColleGame.Current.MasterInfo.Ships[Convert.ToInt32(rpReader["flagship"])];
            HeadquarterLevel = Convert.ToInt32(rpReader["hq_level"]);
        }
    }
}
