using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ConstructionRecord : RecordBase
    {
        public static Subject<RecordItem> NewRecord { get; } = new Subject<RecordItem>();

        public override string GroupName => "construction";

        internal ConstructionRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ConstructionDock.NewConstruction.Subscribe(InsertRecord));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS construction(" +
                    "time INTEGER PRIMARY KEY NOT NULL, " +
                    "ship INTEGER NOT NULL, " +
                    "fuel INTEGER NOT NULL, " +
                    "bullet INTEGER NOT NULL, " +
                    "steel INTEGER NOT NULL, " +
                    "bauxite INTEGER NOT NULL, " +
                    "dev_material INTEGER NOT NULL, " +
                    "flagship INTEGER NOT NULL, " +
                    "hq_level INTEGER NOT NULL, " +
                    "is_lsc BOOLEAN NOT NULL," +
                    "empty_dock INTEGER)";

                rCommand.ExecuteNonQuery();
            }
        }

        public async Task<List<RecordItem>> LoadRecordsAsync()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM construction ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rResult = new List<RecordItem>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rResult.Add(new RecordItem(rReader));

                    return rResult;
                }
            }
        }

        internal void InsertRecord(ConstructionDock rpData)
        {
            var rPort = KanColleGame.Current.Port;
            var rShip = rpData.Ship;
            var rSecretaryShip = rPort.Fleets[1].Ships[0].Info;
            var rHeadquarterLevel = rPort.Admiral.Level;
            var rEmptyDockCount = !rpData.IsLargeShipConstruction.Value ? (int?)null : rPort.ConstructionDocks.Values.Count(r => r.State == ConstructionDockState.Idle);

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO construction(time, ship, fuel, bullet, steel, bauxite, dev_material, flagship, hq_level, is_lsc, empty_dock) " +
                    "VALUES(strftime('%s', 'now'), @ship, @fuel, @bullet, @steel, @bauxite, @dev_material, @flagship, @hq_level, @is_lsc, @empty_dock);";
                rCommand.Parameters.AddWithValue("@ship", rShip.ID);
                rCommand.Parameters.AddWithValue("@fuel", rpData.FuelConsumption);
                rCommand.Parameters.AddWithValue("@bullet", rpData.BulletConsumption);
                rCommand.Parameters.AddWithValue("@steel", rpData.SteelConsumption);
                rCommand.Parameters.AddWithValue("@bauxite", rpData.BauxiteConsumption);
                rCommand.Parameters.AddWithValue("@dev_material", rpData.DevelopmentMaterialConsumption);
                rCommand.Parameters.AddWithValue("@flagship", rSecretaryShip.ID);
                rCommand.Parameters.AddWithValue("@hq_level", rHeadquarterLevel);
                rCommand.Parameters.AddWithValue("@is_lsc", rpData.IsLargeShipConstruction);
                rCommand.Parameters.AddWithValue("@empty_dock", rEmptyDockCount);

                rCommand.ExecuteNonQuery();
            }

            NewRecord.OnNext(new RecordItem(rShip, rpData.FuelConsumption, rpData.BulletConsumption, rpData.SteelConsumption, rpData.BauxiteConsumption, rpData.DevelopmentMaterialConsumption,
                rSecretaryShip, rHeadquarterLevel, rEmptyDockCount));
        }

        public class RecordItem
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

            internal RecordItem(DbDataReader rpReader)
            {
                Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["time"])).LocalDateTime.ToString();
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
            internal RecordItem(ShipInfo rpShip, int rpFuelConsumption, int rpBulletConsumption, int rpSteelConsumption, int rpBauxiteConsumption, int rpDevelopmentMaterialConsumption, ShipInfo rpSecretaryShip, int rpHeadquarterLevel, int? rpEmptyDockCount)
            {
                Time = DateTime.Now.ToString();
                Ship = rpShip;

                FuelConsumption = rpFuelConsumption;
                BulletConsumption = rpBulletConsumption;
                SteelConsumption = rpSteelConsumption;
                BauxiteConsumption = rpBauxiteConsumption;
                DevelopmentMaterialConsumption = rpDevelopmentMaterialConsumption;

                SecretaryShip = rpSecretaryShip;
                HeadquarterLevel = rpHeadquarterLevel;

                EmptyDockCount = rpEmptyDockCount;
            }
        }
    }
}
