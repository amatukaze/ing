using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class DevelopmentRecord : RecordBase
    {
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

        void InsertRecord(RawEquipmentDevelopment rpData, int rpFuel, int rpBullet, int rpSteel, int rpBauxite)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO development(time, equipment, fuel, bullet, steel, bauxite, flagship, hq_level) " +
                    "VALUES(strftime('%s', 'now'), @equipment, @fuel, @bullet, @steel, @bauxite, @flagship, @hq_level);";
                rCommand.Parameters.AddWithValue("@equipment", rpData.Success ? rpData.Result.EquipmentID : (int?)null);
                rCommand.Parameters.AddWithValue("@fuel", rpFuel);
                rCommand.Parameters.AddWithValue("@bullet", rpBullet);
                rCommand.Parameters.AddWithValue("@steel", rpSteel);
                rCommand.Parameters.AddWithValue("@bauxite", rpBauxite);
                rCommand.Parameters.AddWithValue("@flagship", KanColleGame.Current.Port.Fleets[1].Ships[0].Info.ID);
                rCommand.Parameters.AddWithValue("@hq_level", KanColleGame.Current.Port.Admiral.Level);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
