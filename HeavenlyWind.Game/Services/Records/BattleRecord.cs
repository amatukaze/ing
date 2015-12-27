using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class BattleRecord : RecordBase
    {
        public override string GroupName => "battle";

        public override int Version => 2;

        internal BattleRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            var rBattleApis = new[]
            {
                "api_req_sortie/battle",
                "api_req_battle_midnight/sp_midnight",
                "api_req_sortie/airbattle",
                "api_req_combined_battle/airbattle",
                "api_req_combined_battle/battle",
                "api_req_combined_battle/battle_water",
                "api_req_combined_battle/sp_midnight",
                "api_req_battle_midnight/battle",
                "api_req_combined_battle/midnight_battle",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rBattleApis, Process));

            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            DisposableObjects.Add(SessionService.Instance.Subscribe(rBattleResultApis, r => ProcessResult((RawBattleResult)r.Data)));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS battle_count(" +
                    "map INTEGER NOT NULL, " +
                    "cell INTEGER NOT NULL, " +
                    "count INTEGER NOT NULL DEFAULT 0, " +
                    "CONSTRAINT [] PRIMARY KEY(map, cell)) WITHOUT ROWID;"+

                "CREATE TABLE IF NOT EXISTS battle(" +
                    "id INTEGER PRIMARY KEY, " +
                    "rank INTEGER, " +
                    "dropped_ship INTEGER);" +

                "CREATE TABLE IF NOT EXISTS battle_dropped_item(" +
                    "id INTEGER PRIMARY KEY NOT NULL REFERENCES battle(id), " +
                    "item INTEGER NOT NULL);" +

                "CREATE TABLE IF NOT EXISTS battle_bonus_point(" +
                    "id INTEGER PRIMARY KEY NOT NULL REFERENCES battle(id), " +
                    "time INTEGER NOT NULL, " +
                    "point INTEGER NOT NULL);" +

                "CREATE TABLE IF NOT EXISTS battle_participant(" +
                    "battle INTEGER NOT NULL REFERENCES battle(id), " +
                    "ship INTEGER NOT NULL, " +
                    "level INTEGER NOT NULL, " +
                    "CONSTRAINT [] PRIMARY KEY(battle, ship)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }

        void Process(ApiData rpData)
        {
            if (rpData.Api != "api_req_battle_midnight/battle" && rpData.Api != "api_req_combined_battle/midnight_battle")
                UpdateCount();
        }
        void UpdateCount()
        {
            var rSortieInfo = KanColleGame.Current.Sortie;
            var rMap = rSortieInfo.Map.ID;
            var rCell = rSortieInfo.Cell.InternalID;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "BEGIN TRANSACTION;" +
                    "INSERT OR IGNORE INTO battle_count(map, cell) VALUES(@map, @cell);" +
                    "UPDATE battle_count SET count = count + 1 WHERE map = @map AND cell = @cell;" +
                    "INSERT INTO battle(id) VALUES(@id);" +
                    "COMMIT;";
                rCommand.Parameters.AddWithValue("@map", rMap);
                rCommand.Parameters.AddWithValue("@cell", rCell);
                rCommand.Parameters.AddWithValue("@id", ((BattleEvent)KanColleGame.Current.Sortie.Cell.Event).Battle.ID);

                rCommand.ExecuteNonQuery();
            }
        }

        void ProcessResult(RawBattleResult rpData)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE battle SET rank = @rank, dropped_ship = @dropped_ship WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", ((BattleEvent)KanColleGame.Current.Sortie.Cell.Event).Battle.ID);
                rCommand.Parameters.AddWithValue("@rank", (int)rpData.Rank);
                rCommand.Parameters.AddWithValue("@dropped_ship", rpData.DroppedShip?.ID);

                if (rpData.DroppedItem != null)
                {
                    rCommand.CommandText += "INSERT INTO battle_dropped_item(id, item) VALUES(@id, @item);";
                    rCommand.Parameters.AddWithValue("@item", rpData.DroppedItem.ID);
                }

                if (rpData.ExtraOperationBonusPoint != 0)
                {
                    rCommand.CommandText += "INSERT INTO battle_bonus_point(id, time, point) VALUES(@id, strftime('%s', 'now'), @point);";
                    rCommand.Parameters.AddWithValue("@point", rpData.ExtraOperationBonusPoint);
                }

                rCommand.ExecuteNonQuery();
            }
        }

    }
}
