using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class BattleRecords : RecordsGroup
    {
        public override string GroupName => "battle";

        public override int Version => 3;

        internal BattleRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            DisposableObjects.Add(ApiService.Subscribe(rBattleResultApis, r => ProcessResult((RawBattleResult)r.Data)));
        }

        protected override void UpgradeFromOldVersionPreprocessStep(int rpOldVersion)
        {
            if (rpOldVersion < 3)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "DROP TABLE IF EXISTS battle_count;";

                    rCommand.ExecuteNonQuery();
                }
        }
        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS battle(" +
                        "id INTEGER PRIMARY KEY, " +
                        "rank INTEGER, " +
                        "dropped_ship INTEGER);" +

                    "CREATE TABLE IF NOT EXISTS battle_dropped_item(" +
                        "id INTEGER PRIMARY KEY NOT NULL REFERENCES battle(id), " +
                        "item INTEGER NOT NULL);" +

                    "CREATE TABLE IF NOT EXISTS battle_participant(" +
                        "battle INTEGER NOT NULL REFERENCES battle(id), " +
                        "ship INTEGER NOT NULL, " +
                        "level INTEGER NOT NULL, " +
                        "PRIMARY KEY(battle, ship)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }

        void ProcessResult(RawBattleResult rpData)
        {
            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO battle(id, rank, dropped_ship) VALUES(@id, @rank, @dropped_ship);";
                rCommand.Parameters.AddWithValue("@id", BattleInfo.Current.ID);
                rCommand.Parameters.AddWithValue("@rank", (int)rpData.Rank);
                rCommand.Parameters.AddWithValue("@dropped_ship", rpData.DroppedShip?.ID);

                if (rpData.DroppedItem != null)
                {
                    rCommand.CommandText += " INSERT INTO battle_dropped_item(id, item) VALUES(@id, @item);";
                    rCommand.Parameters.AddWithValue("@item", rpData.DroppedItem.ID);
                }

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }

    }
}
