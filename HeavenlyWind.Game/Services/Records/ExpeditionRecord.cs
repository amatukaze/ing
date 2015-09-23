using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ExpeditionRecord : RecordBase
    {
        public override string TableName => "expedition";
        public override int Version => 1;

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
                    "CONSTRAINT [] PRIMARY KEY(ship, expedition)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
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
                rCommand.Parameters.AddWithValue("@item1", rpData.Item1 != null ? rpData.Item1.ID : (int?)null);
                rCommand.Parameters.AddWithValue("@item1_count", rpData.Item1 != null ? rpData.Item1.Count : (int?)null);
                rCommand.Parameters.AddWithValue("@item2", rpData.Item2 != null ? rpData.Item2.ID : (int?)null);
                rCommand.Parameters.AddWithValue("@item2_count", rpData.Item2 != null ? rpData.Item2.Count : (int?)null);

                rCommand.ExecuteNonQuery();
            }
        }
        internal void UpdateCount(int rpExpedition, IEnumerable<int> rpShips)
        {
            foreach (var rShip in rpShips)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "INSERT OR IGNORE INTO expedition_count(ship, expedition) VALUES (@ship, @expedition);" +
                        "UPDATE expedition_count SET count = count + 1 WHERE ship = @ship AND expedition = @expedition;";
                    rCommand.Parameters.AddWithValue("@ship", rShip);
                    rCommand.Parameters.AddWithValue("@expedition", rpExpedition);

                    rCommand.ExecuteNonQuery();
                }
        }
    }
}
