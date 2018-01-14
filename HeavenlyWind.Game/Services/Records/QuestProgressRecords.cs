using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class QuestProgressRecords : RecordsGroup
    {
        public override string GroupName => "quest";

        HybridDictionary<int, ProgressInfo> r_Progresses = new HybridDictionary<int, ProgressInfo>(16);

        internal QuestProgressRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_quest/clearitemget", r => DeleteRecord(int.Parse(r.Parameters["api_quest_id"]))));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS quest(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "name TEXT NOT NULL, " +
                    "description TEXT NOT NULL, " +
                    "type INTEGER NOT NULL, " +
                    "state INTEGER NOT NULL, " +
                    "progress INTEGER NOT NULL, " +
                    "update_time INTEGER NOT NULL);";

                rCommand.ExecuteNonQuery();
            }
        }

        internal HybridDictionary<int, ProgressInfo> Reload()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM quest;";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rID = Convert.ToInt32(rReader["id"]);
                        var rResetType = (QuestType)Convert.ToInt32(rReader["type"]);
                        var rState = (QuestState)Convert.ToInt32(rReader["state"]);
                        var rProgress = Convert.ToInt32(rReader["progress"]);
                        var rUpdateTime = DateTimeUtil.FromUnixTime(Convert.ToInt64(rReader["update_time"]));

                        ProgressInfo rInfo;
                        if (!r_Progresses.TryGetValue(rID, out rInfo))
                            r_Progresses.Add(rID, rInfo = new ProgressInfo(rID, rResetType, rState, rProgress, rUpdateTime));
                        else
                        {
                            rInfo.State = rState;
                            rInfo.Progress = rProgress;
                            rInfo.UpdateTime = rUpdateTime;
                        }
                    }
            }

            return r_Progresses;
        }

        internal void InsertRecord(RawQuest rpQuest, int rpProgress)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR REPLACE INTO quest(id, name, description, type, state, progress, update_time) " +
                    "VALUES(@id, @name, @description, @type, @state, @progress, strftime('%s', 'now'));";
                rCommand.Parameters.AddWithValue("@id", rpQuest.ID);
                rCommand.Parameters.AddWithValue("@name", rpQuest.Name);
                rCommand.Parameters.AddWithValue("@description", rpQuest.Description);
                rCommand.Parameters.AddWithValue("@type", rpQuest.Type);
                rCommand.Parameters.AddWithValue("@state", (int)rpQuest.State);
                rCommand.Parameters.AddWithValue("@progress", rpProgress);

                rCommand.ExecuteNonQuery();
            }
        }

        internal void UpdateResetType(ProgressInfo rpProgress)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE quest SET type = @type WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpProgress.Quest.ID);
                rCommand.Parameters.AddWithValue("@type", rpProgress.ResetType);

                rCommand.ExecuteNonQuery();
            }
        }
        internal void UpdateState(ProgressInfo rpProgress)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE quest SET state = @state, update_time = strftime('%s', 'now') WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpProgress.Quest.ID);
                rCommand.Parameters.AddWithValue("@state", rpProgress.State);

                rCommand.ExecuteNonQuery();
            }
        }
        internal void UpdateProgress(ProgressInfo rpProgress)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE quest SET progress = @progress, update_time = strftime('%s', 'now') WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpProgress.Quest.ID);
                rCommand.Parameters.AddWithValue("@progress", rpProgress.Progress);

                rCommand.ExecuteNonQuery();
            }
        }

        internal void DeleteRecord(int rpID)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "DELETE FROM quest WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpID);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
