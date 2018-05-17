using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    class DevelopmentRecords : RecordsGroup
    {
        public override string GroupName => "development";

        internal DevelopmentRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_kousyou/createitem", r =>
            {
                var rFuelConsumption = int.Parse(r.Parameters["api_item1"]);
                var rBulletConsumption = int.Parse(r.Parameters["api_item2"]);
                var rSteelConsumption = int.Parse(r.Parameters["api_item3"]);
                var rBauxiteConsumption = int.Parse(r.Parameters["api_item4"]);

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
        }
    }
}
