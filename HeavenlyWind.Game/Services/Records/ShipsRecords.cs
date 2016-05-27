using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ShipsRecords : RecordsGroup
    {
        public override string GroupName => "ships";

        internal ShipsRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS ships(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "ship_id INTEGER NOT NULL, " +
                    "absent BOOLEAN NOT NULL DEFAULT 0);";

                rCommand.ExecuteNonQuery();
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
