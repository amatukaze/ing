using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class DevelopmentRecord : RecordBase
    {
        public static Subject<RecordItem> NewRecord { get; } = new Subject<RecordItem>();

        public override string GroupName => "development";

        internal DevelopmentRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_kousyou/createitem", r =>
            {
                var rFuelConsumption = int.Parse(r.Requests["api_item1"]);
                var rBulletConsumption = int.Parse(r.Requests["api_item2"]);
                var rSteelConsumption = int.Parse(r.Requests["api_item3"]);
                var rBauxiteConsumption = int.Parse(r.Requests["api_item4"]);

                InsertRecord((RawEquipmentDevelopment)r.Data, rFuelConsumption, rBulletConsumption, rSteelConsumption, rBauxiteConsumption);
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS development(" +
                    "time INTEGER PRIMARY KEY NOT NULL, " +
                    "equipment INTEGER, " +
                    "fuel INTEGER NOT NULL, " +
                    "bullet INTEGER NOT NULL, " +
                    "steel INTEGER NOT NULL, " +
                    "bauxite INTEGER NOT NULL, " +
                    "flagship INTEGER NOT NULL, " +
                    "hq_level INTEGER NOT NULL)";

                rCommand.ExecuteNonQuery();
            }
        }

        public async Task<List<RecordItem>> LoadRecordsAsync()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM development ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rResult = new List<RecordItem>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rResult.Add(new RecordItem(rReader));

                    return rResult;
                }
            }
        }

        void InsertRecord(RawEquipmentDevelopment rpData, int rpFuelConsumption, int rpBulletConsumption, int rpSteelConsumption, int rpBauxiteConsumption)
        {
            var rEquipmentID = rpData.Success ? rpData.Result.EquipmentID : (int?)null;
            var rSecretaryShip = KanColleGame.Current.Port.Fleets[1].Ships[0].Info;
            var rHeadquarterLevel = KanColleGame.Current.Port.Admiral.Level;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO development(time, equipment, fuel, bullet, steel, bauxite, flagship, hq_level) " +
                    "VALUES(strftime('%s', 'now'), @equipment, @fuel, @bullet, @steel, @bauxite, @flagship, @hq_level);";
                rCommand.Parameters.AddWithValue("@equipment", rEquipmentID);
                rCommand.Parameters.AddWithValue("@fuel", rpFuelConsumption);
                rCommand.Parameters.AddWithValue("@bullet", rpBulletConsumption);
                rCommand.Parameters.AddWithValue("@steel", rpSteelConsumption);
                rCommand.Parameters.AddWithValue("@bauxite", rpBauxiteConsumption);
                rCommand.Parameters.AddWithValue("@flagship", rSecretaryShip.ID);
                rCommand.Parameters.AddWithValue("@hq_level", KanColleGame.Current.Port.Admiral.Level);

                rCommand.ExecuteNonQuery();
            }

            NewRecord.OnNext(new RecordItem(rEquipmentID, rpFuelConsumption, rpBulletConsumption, rpSteelConsumption, rpBauxiteConsumption, rSecretaryShip, rHeadquarterLevel));
        }

        public class RecordItem
        {
            public string Time { get; }

            public EquipmentInfo Equipment { get; }

            public int FuelConsumption { get; }
            public int BulletConsumption { get; }
            public int SteelConsumption { get; }
            public int BauxiteConsumption { get; }

            public ShipInfo SecretaryShip { get; }
            public int HeadquarterLevel { get; }

            internal RecordItem(DbDataReader rpReader)
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
            internal RecordItem(int? rpEquipmentID, int rpFuelConsumption, int rpBulletConsumption, int rpSteelConsumption, int rpBauxiteConsumption, ShipInfo rpSecretaryShip, int rpHeadquarterLevel)
            {
                Time = DateTime.Now.ToString();
                if (rpEquipmentID.HasValue)
                    Equipment = KanColleGame.Current.MasterInfo.Equipment[rpEquipmentID.Value];

                FuelConsumption = rpFuelConsumption;
                BulletConsumption = rpBulletConsumption;
                SteelConsumption = rpSteelConsumption;
                BauxiteConsumption = rpBauxiteConsumption;

                SecretaryShip = rpSecretaryShip;
                HeadquarterLevel = rpHeadquarterLevel;
            }
        }
    }
}
