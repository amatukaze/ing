using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ExpeditionRecords : RecordsBase
    {
        public override string GroupName => "expedition";
        public override int Version => 2;

        internal ExpeditionRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_mission/result", r =>
            {
                using (var rTransaction = Connection.BeginTransaction())
                {
                    var rData = (RawExpeditionResult)r.Data;

                    var rFleet = KanColleGame.Current.Port.Fleets[int.Parse(r.Parameters["api_deck_id"])];
                    var rExpedition = rFleet.ExpeditionStatus.Expedition;

                    InsertRecord(rExpedition.ID, rData);
                    UpdateCount(rExpedition.ID, rData.Ships.Skip(1));

                    rTransaction.Commit();
                }
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS expedition(" +
                    "time INTEGER PRIMARY KEY, " +
                    "expedition INTEGER, " +
                    "result INTEGER, " +
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
        protected override void UpgradeFromOldVersion(int rpOldVersion)
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

        internal void InsertRecord(int rpExpedition, RawExpeditionResult rpData)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                var rMaterials = rpData.Result != 0 ? rpData.Materials.ToObject<int[]>() : null;

                rCommand.CommandText = "INSERT INTO expedition(time, result, expedition, fuel, bullet, steel, bauxite, item1, item1_count, item2, item2_count) " +
                    "VALUES(strftime('%s', 'now'), @result, @expedition, @fuel, @bullet, @steel, @bauxite, @item1, @item1_count, @item2, @item2_count);";
                rCommand.Parameters.AddWithValue("@result", (int)rpData.Result);
                rCommand.Parameters.AddWithValue("@expedition", rpExpedition);
                rCommand.Parameters.AddWithValue("@fuel", rMaterials != null ? rMaterials[0] : (int?)null);
                rCommand.Parameters.AddWithValue("@bullet", rMaterials != null ? rMaterials[1] : (int?)null);
                rCommand.Parameters.AddWithValue("@steel", rMaterials != null ? rMaterials[2] : (int?)null);
                rCommand.Parameters.AddWithValue("@bauxite", rMaterials != null ? rMaterials[3] : (int?)null);

                rCommand.Parameters.AddWithValue("@item1", GetItemID(rpData.RewardItems[0], rpData.Item1?.ID));
                rCommand.Parameters.AddWithValue("@item1_count", rpData.Item1 != null ? rpData.Item1.Count : (int?)null);
                rCommand.Parameters.AddWithValue("@item2", GetItemID(rpData.RewardItems[1], rpData.Item2?.ID));
                rCommand.Parameters.AddWithValue("@item2_count", rpData.Item2 != null ? rpData.Item2.Count : (int?)null);

                rCommand.ExecuteNonQuery();
            }
        }
        int? GetItemID(int rpFlag, int? rpItemID)
        {
            if (rpFlag == 0)
                return null;

            return rpItemID <= 0 ? rpFlag : rpItemID;
        }
        internal void UpdateCount(int rpExpedition, IEnumerable<int> rpShips)
        {
            foreach (var rShip in rpShips)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "INSERT OR IGNORE INTO expedition_count(ship, expedition) VALUES(@ship, @expedition);" +
                        "UPDATE expedition_count SET count = count + 1 WHERE ship = @ship AND expedition = @expedition;";
                    rCommand.Parameters.AddWithValue("@ship", rShip);
                    rCommand.Parameters.AddWithValue("@expedition", rpExpedition);

                    rCommand.ExecuteNonQuery();
                }
        }
    }
}
