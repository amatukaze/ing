using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Data.Common;

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

        internal ConstructionRecord(DbDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(Convert.ToInt64(rpReader["time"])).LocalDateTime.ToString();
            Ship = KanColleGame.Current.MasterInfo.Ships[Convert.ToInt32(rpReader["ship"])];

            FuelConsumption = Convert.ToInt32(rpReader["fuel"]);
            BulletConsumption = Convert.ToInt32(rpReader["bullet"]);
            SteelConsumption = Convert.ToInt32(rpReader["steel"]);
            BauxiteConsumption = Convert.ToInt32(rpReader["bauxite"]);
            DevelopmentMaterialConsumption = Convert.ToInt32(rpReader["dev_material"]);

            SecretaryShip = KanColleGame.Current.MasterInfo.Ships[Convert.ToInt32(rpReader["flagship"])];
            HeadquarterLevel = Convert.ToInt32(rpReader["hq_level"]);

            var rEmptyDockCount = rpReader["empty_dock"];
            if (rEmptyDockCount != DBNull.Value)
                EmptyDockCount = Convert.ToInt32(rEmptyDockCount);
        }
    }
}
