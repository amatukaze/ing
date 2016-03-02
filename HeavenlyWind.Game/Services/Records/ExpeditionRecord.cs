using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ExpeditionRecord : RecordBase
    {
        public override string GroupName => "expedition";
        public override int Version => 2;

        internal ExpeditionRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_mission/result", r =>
            {
                using (var rTransaction = Connection.BeginTransaction())
                {
                    var rData = (RawExpeditionResult)r.Data;
                    var rExpedition = KanColleGame.Current.MasterInfo.Expeditions.Values.Single(rpExpedition => rpExpedition.Name == rData.Name).ID;

                    InsertRecord(rExpedition, rData);
                    UpdateCount(rExpedition, rData.Ships.Skip(1));

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

        public async Task<List<RecordItem>> LoadRecordsAsync()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM expedition ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rResult = new List<RecordItem>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rResult.Add(new RecordItem(rReader));

                    return rResult;
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

        public class RecordItem
        {
            public string Time { get; }

            public ExpeditionInfo Expedition { get; }

            public ExpeditionResult Result { get; }

            public int? Fuel { get; }
            public int? Bullet { get; }
            public int? Steel { get; }
            public int? Bauxite { get; }

            public ItemInfo Item1 { get; }
            public int? Item1Count { get; }
            public ItemInfo Item2 { get; }
            public int? Item2Count { get; }

            internal RecordItem(DbDataReader rpReader)
            {
                Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["time"])).LocalDateTime.ToString();

                ExpeditionInfo rExpedition;
                if (KanColleGame.Current.MasterInfo.Expeditions.TryGetValue(Convert.ToInt32(rpReader["expedition"]), out rExpedition))
                    Expedition = rExpedition;
                else
                    Expedition = ExpeditionInfo.Dummy;

                Result = (ExpeditionResult)Convert.ToInt32(rpReader["result"]);

                if (Result == ExpeditionResult.Failure)
                    return;

                Fuel = Convert.ToInt32(rpReader["fuel"]);
                Bullet = Convert.ToInt32(rpReader["bullet"]);
                Steel = Convert.ToInt32(rpReader["steel"]);
                Bauxite = Convert.ToInt32(rpReader["bauxite"]);

                var rItem1 = rpReader["item1"];
                if (rItem1 != DBNull.Value)
                {
                    var rItem1ID = Convert.ToInt32(rItem1);
                    if (rItem1ID != -1)
                        Item1 = KanColleGame.Current.MasterInfo.Items[rItem1ID];
                    Item1Count = Convert.ToInt32(rpReader["item1_count"]);
                }
                var rItem2 = rpReader["item2"];
                if (rItem2 != DBNull.Value)
                {
                    var rItem2ID = Convert.ToInt32(rItem2);
                    if (rItem2ID != -1)
                        Item2 = KanColleGame.Current.MasterInfo.Items[rItem2ID];
                    Item2Count = Convert.ToInt32(rpReader["item2_count"]);
                }
            }
        }
    }
}
