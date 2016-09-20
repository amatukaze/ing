using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ExperienceRecords : RecordsGroup
    {
        static object r_ThreadSyncLock = new object();

        public override string GroupName => "experience";

        int r_Admiral;
        Dictionary<int, int> r_Ships = new Dictionary<int, int>(100);

        internal ExperienceRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_port/port", r =>
            {
                var rPort = KanColleGame.Current.Port;

                lock (r_ThreadSyncLock)
                    using (var rTransaction = Connection.BeginTransaction())
                    {
                        if (r_Admiral != rPort.Admiral.Experience)
                        {
                            r_Admiral = rPort.Admiral.Experience;
                            InsertAdmiralRecord(r.Timestamp, rPort.Admiral.Experience);
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

                        rTransaction.Commit();
                    }
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
            using (var rCommand = Connection.CreateCommand())
            {
                var rBuilder = new StringBuilder(128);

                rBuilder.Append("INSERT INTO ship_experience(id, time, experience) VALUES");
                for (var i = 0; i < rpShips.Count; i++)
                {
                    if (i > 0)
                        rBuilder.Append(", ");

                    var rShip = rpShips[i];
                    rBuilder.Append($"({rShip.ID}, {rpTimestamp}, {rShip.Experience})");
                }
                rBuilder.Append(';');

                rCommand.CommandText = rBuilder.ToString();
                rCommand.ExecuteNonQuery();
            }
        }
    }
}
