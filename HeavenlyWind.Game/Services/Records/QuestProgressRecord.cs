using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class QuestProgressRecord : RecordBase
    {
        public override string GroupName => "quest_progress";

        Dictionary<int, ProgressInfo> r_Progresses = new Dictionary<int, ProgressInfo>(16);

        internal QuestProgressRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_quest/clearitemget", r => DeleteRecord(int.Parse(r.Requests["api_quest_id"]))));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS quest_progress(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "state INTEGER NOT NULL, " +
                    "progress INTEGER NOT NULL, " +
                    "update_time INTEGER NOT NULL);";

                rCommand.ExecuteNonQuery();
            }
        }

        internal Dictionary<int, ProgressInfo> Reload()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM quest_progress;";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rID = Convert.ToInt32(rReader["id"]);
                        var rState = (QuestState)Convert.ToInt32(rReader["state"]);
                        var rProgress = Convert.ToInt32(rReader["progress"]);
                        var rUpdateTime = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rReader["update_time"]));

                        ProgressInfo rInfo;
                        if (!r_Progresses.TryGetValue(rID, out rInfo))
                            r_Progresses.Add(rID, new ProgressInfo(rID, rState, rProgress, rUpdateTime));
                        else
                        {
                            rInfo.State = rState;
                            rInfo.Progress = rProgress;
                            rInfo.UpdateTime = rUpdateTime;
                            rInfo.IsDirty = false;
                        }
                    }
            }

            return r_Progresses;
        }

        void UpdateRecords()
        {
            using (var rTransaction = Connection.BeginTransaction())
            {
                foreach (var rProgress in r_Progresses.Values.Where(r => r.IsDirty))
                {
                    using (var rCommand = Connection.CreateCommand())
                    {
                        rCommand.CommandText = "UPDATE quest_progress SET progress = @progress, update_time = strftime('%s', 'now') WHERE id = @id;";
                        rCommand.Parameters.AddWithValue("@id", rProgress.Quest.ID);
                        rCommand.Parameters.AddWithValue("@progress", rProgress.Progress);

                        rCommand.ExecuteNonQuery();
                    }

                    rProgress.IsDirty = false;
                }

                rTransaction.Commit();
            }
        }
        void DeleteRecord(int rpID)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "DELETE FROM quest_progress WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpID);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
