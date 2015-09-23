using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ShipsRecord : RecordBase
    {
        public override string TableName => "ships";
        public override int Version => 1;

        HashSet<int> r_Ships = new HashSet<int>();

        internal ShipsRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_port/port", _ =>
            {
                using (var rTransaction = Connection.BeginTransaction())
                {
                    foreach (var rShip in KanColleGame.Current.Port.Ships.Values.Where(r => r.Experience > 0))
                        if (r_Ships.Add(rShip.ID))
                            InsertRecord(rShip);

                    rTransaction.Commit();
                }
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS ships(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "ship_id INTEGER NOT NULL, " +
                    "absent BOOLEAN NOT NULL DEFAULT FALSE);";

                rCommand.ExecuteNonQuery();
            }
        }
        protected override void Load()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT id FROM ships WHERE NOT(absent);";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                        r_Ships.Add(rReader.GetInt32(0));
            }
        }

        void InsertRecord(Ship rShip)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO ships(id, ship_id) " +
                    "VALUES(@id, @ship_id);";
                rCommand.Parameters.AddWithValue("@id", rShip.ID);
                rCommand.Parameters.AddWithValue("@ship_id", rShip.Info.ID);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
