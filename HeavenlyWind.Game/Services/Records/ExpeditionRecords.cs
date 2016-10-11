using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ExpeditionRecords : RecordsGroup
    {
        public override string GroupName => "expedition";
        public override int Version => 2;

        internal ExpeditionRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_mission/result", ProcessResult));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS expedition(" +
                        "time INTEGER NOT NULL PRIMARY KEY, " +
                        "expedition INTEGER NOT NULL, " +
                        "result INTEGER NOT NULL, " +
                        "fuel INTEGER, " +
                        "bullet INTEGER, " +
                        "steel INTEGER, " +
                        "bauxite INTEGER, " +
                        "item1 INTEGER, " +
                        "item1_count INTEGER, " +
                        "item2 INTEGER, " +
                        "item2_count INTEGER);" +

                    "CREATE TABLE IF NOT EXISTS expedition_count(" +
                        "ship INTEGER NOT NULL, " +
                        "expedition INTEGER NOT NULL, " +
                        "count INTEGER NOT NULL DEFAULT 0, " +
                        "PRIMARY KEY(ship, expedition)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }
        protected override void UpgradeFromOldVersionPreprocessStep(int rpOldVersion)
        {
            if (rpOldVersion < 2)
            {
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "SELECT expedition FROM expedition WHERE item1 = -1 OR item2 = -1 GROUP BY expedition;";

                    using (var rReader = rCommand.ExecuteReader())
                        while (rReader.Read())
                        {
                            var rExpedition = KanColleGame.Current.MasterInfo.Expeditions[Convert.ToInt32(rReader["expedition"])];

                            using (var rUpdateCommand = Connection.CreateCommand())
                            {
                                rUpdateCommand.CommandText =
                                    "UPDATE expedition SET item1 = @item1 WHERE expedition = @expedition AND item1 = -1;" +
                                    "UPDATE expedition SET item2 = @item2 WHERE expedition = @expedition AND item2 = -1;";
                                rUpdateCommand.Parameters.AddWithValue("@expedition", rExpedition.ID);
                                rUpdateCommand.Parameters.AddWithValue("@item1", rExpedition.RewardItem1ID);
                                rUpdateCommand.Parameters.AddWithValue("@item2", rExpedition.RewardItem2ID);

                                rUpdateCommand.ExecuteNonQuery();
                            }
                        }
                }
            }
        }

        void ProcessResult(ApiInfo rpInfo)
        {
            var rData = (RawExpeditionResult)rpInfo.Data;
            var rFleet = KanColleGame.Current.Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];
            var rExpedition = rFleet.ExpeditionStatus.Expedition ?? KanColleGame.Current.MasterInfo.GetExpeditionFromName(rData.Name);
            var rMaterials = rData.Result != 0 ? rData.Materials.ToObject<int[]>() : null;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO expedition(time, result, expedition, fuel, bullet, steel, bauxite, item1, item1_count, item2, item2_count) " +
                    "VALUES(@time, @result, @expedition, @fuel, @bullet, @steel, @bauxite, @item1, @item1_count, @item2, @item2_count);";
                rCommand.Parameters.AddWithValue("@time", rpInfo.Timestamp);
                rCommand.Parameters.AddWithValue("@result", (int)rData.Result);
                rCommand.Parameters.AddWithValue("@expedition", rExpedition);
                rCommand.Parameters.AddWithValue("@fuel", rMaterials != null ? rMaterials[0] : (int?)null);
                rCommand.Parameters.AddWithValue("@bullet", rMaterials != null ? rMaterials[1] : (int?)null);
                rCommand.Parameters.AddWithValue("@steel", rMaterials != null ? rMaterials[2] : (int?)null);
                rCommand.Parameters.AddWithValue("@bauxite", rMaterials != null ? rMaterials[3] : (int?)null);

                rCommand.Parameters.AddWithValue("@item1", GetItemID(rData.RewardItems[0], rData.Item1?.ID));
                rCommand.Parameters.AddWithValue("@item1_count", rData.Item1 != null ? rData.Item1.Count : (int?)null);
                rCommand.Parameters.AddWithValue("@item2", GetItemID(rData.RewardItems[1], rData.Item2?.ID));
                rCommand.Parameters.AddWithValue("@item2_count", rData.Item2 != null ? rData.Item2.Count : (int?)null);

                rCommand.ExecuteNonQuery();
            }
        }
        int? GetItemID(int rpFlag, int? rpItemID)
        {
            if (rpFlag == 0)
                return null;

            return rpItemID <= 0 ? rpFlag : rpItemID;
        }
    }
}
