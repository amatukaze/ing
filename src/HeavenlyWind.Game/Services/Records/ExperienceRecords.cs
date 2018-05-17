using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    class ExperienceRecords : RecordsGroup
    {
        public override string GroupName => "experience";

        int r_Admiral;
        SortedList<int, int> r_Ships = new SortedList<int, int>(100);

        internal ExperienceRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_port/port", r =>
            {
                var rPort = KanColleGame.Current.Port;
                var rAdmiral = rPort.Admiral;

                if (r_Admiral != rAdmiral.Experience)
                {
                    r_Admiral = rAdmiral.Experience;
                    InsertAdmiralRecord(r.Timestamp, rAdmiral.Experience);
                }

                var rShips = new List<Ship>(25);
                foreach (var rShip in rPort.Ships.Values.Where(rpShip => rpShip.Experience > 0))
                {
                    int rOldExperience;
                    if (!r_Ships.TryGetValue(rShip.ID, out rOldExperience))
                        r_Ships.Add(rShip.ID, rShip.Experience);
                    else
                        r_Ships[rShip.ID] = rShip.Experience;

                    if (rOldExperience != rShip.Experience)
                        rShips.Add(rShip);
                }

                if (rShips.Count > 0)
                    InsertShipExperience(r.Timestamp, rShips);
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS admiral_experience(" +
                        "time INTEGER PRIMARY KEY NOT NULL, " +
                        "experience INTEGER NOT NULL);" +

                    "CREATE TABLE IF NOT EXISTS ship_experience(" +
                        "id INTEGER NOT NULL, " +
                        "time INTEGER NOT NULL, " +
                        "experience INTEGER NOT NULL, " +
                        "PRIMARY KEY(id, time)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }
        protected override void Load()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT experience FROM admiral_experience ORDER BY time DESC LIMIT 1;";

                r_Admiral = Convert.ToInt32(rCommand.ExecuteScalar());
            }
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT id, experience FROM ship_experience GROUP BY id;";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rID = Convert.ToInt32(rReader["id"]);
                        var rExperience = Convert.ToInt32(rReader["experience"]);

                        r_Ships.Add(rID, rExperience);
                    }
            }
        }

        void InsertAdmiralRecord(long rpTimestamp, int rpExperience)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO admiral_experience(time, experience) VALUES(@time, @experience);";
                rCommand.Parameters.AddWithValue("@time", rpTimestamp);
                rCommand.Parameters.AddWithValue("@experience", rpExperience);

                rCommand.ExecuteNonQuery();
            }
        }
        void InsertShipExperience(long rpTimestamp, List<Ship> rpShips)
        {
            using (var transaction = Connection.BeginTransaction())
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO ship_experience(id, time, experience) VALUES(@id, @timestamp, @exp);";

                foreach (var ship in rpShips)
                {
                    command.Parameters.AddWithValue("@id", ship.ID);
                    command.Parameters.AddWithValue("@timestamp", rpTimestamp);
                    command.Parameters.AddWithValue("@exp", ship.Experience);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
}
