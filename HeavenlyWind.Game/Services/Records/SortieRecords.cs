using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class SortieRecords : RecordsGroup
    {
        enum ReturnReason { DeadEnd, Retreat, RetreatWithHeavilyDamagedShip, Unexpected }

        public override string GroupName => "sortie";
        public override int Version => 5;

        ReturnReason? r_ReturnReason;

        internal SortieRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_map/start", StartSortie));
            DisposableObjects.Add(ApiService.Subscribe("api_req_map/next", _ => InsertExplorationRecord(SortieInfo.Current)));

            DisposableObjects.Add(ApiService.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, RecordMapHP));

            KanColleGame.Current.ReturnedFromSortie += OnReturnedFromSortie;
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS sortie_map(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "is_event_map BOOLEAN NOT NULL);" +

                "CREATE TABLE IF NOT EXISTS sortie_map_hp(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "difficulty INTEGER NOT NULL, " +
                    "hp INTEGER NOT NULL);" +

                "CREATE TABLE IF NOT EXISTS sortie(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "map INTEGER NOT NULL REFERENCES sortie_map(id), " +
                    "difficulty INTEGER, " +
                    "return_time INTEGER, " +
                    "return_reason INTEGER, " +
                    "map_hp INTEGER);" +

                "CREATE TABLE IF NOT EXISTS sortie_node(" +
                    "map INTEGER NOT NULL REFERENCES sortie_map(id), " +
                    "id INTEGER NOT NULL, " +
                    "type INTEGER NOT NULL, " +
                    "subtype INTEGER NOT NULL, " +
                    "PRIMARY KEY(map, id)) WITHOUT ROWID;" +

                "CREATE TABLE IF NOT EXISTS sortie_detail(" +
                    "id INTEGER NOT NULL REFERENCES sortie(id), " +
                    "step INTEGER NOT NULL, " +
                    "node INTEGER NOT NULL, " +
                    "extra_info INTEGER, " +
                    "PRIMARY KEY(id, step)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }

        protected override void UpgradeFromOldVersionPreprocessStep(int rpOldVersion)
        {
            if (rpOldVersion == 1)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "ALTER TABLE sortie_cell RENAME TO sortie_node;" +
                        "ALTER TABLE sortie_detail RENAME TO sortie_detail_old;" +

                        "CREATE TABLE IF NOT EXISTS sortie_detail(" +
                            "id INTEGER NOT NULL REFERENCES sortie(id), " +
                            "step INTEGER NOT NULL, " +
                            "node INTEGER NOT NULL, " +
                            "extra_info INTEGER, " +
                            "PRIMARY KEY(id, step)) WITHOUT ROWID;" +

                        "INSERT INTO sortie_detail(id, step, node, extra_info) " +
                            "SELECT id, (SELECT COUNT(*) - 1 FROM sortie_detail_old B WHERE B.id = A.id AND B.cell <= A.cell) AS step, cell, extra_info FROM sortie_detail_old A WHERE cell <> -1 " +
                            "UNION " +
                            "SELECT id, (SELECT COUNT(*) FROM sortie_detail_old B WHERE B.id = A.id) AS step, cell, extra_info FROM sortie_detail_old A WHERE cell = -1;" +

                        "DROP TABLE sortie_detail_old;";

                    rCommand.ExecuteNonQuery();
                }

            if (rpOldVersion < 3)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "ALTER TABLE sortie ADD COLUMN difficulty INTEGER;" +
                        "UPDATE sortie SET difficulty = (SELECT node - (node + 2) / 3 * 3 + 3 FROM sortie_detail WHERE id = sortie.id) WHERE (SELECT is_event_map FROM sortie_map WHERE id = sortie.map) = 1;" +
                        "UPDATE sortie_detail SET node = (node + 2) / 3 WHERE (SELECT is_event_map FROM sortie_map WHERE id = (SELECT map FROM sortie WHERE id = sortie_detail.id)) AND node <> -1; ";

                    rCommand.ExecuteNonQuery();
                }

            if (rpOldVersion < 4)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "ALTER TABLE sortie ADD COLUMN return_time INTEGER; " +
                        "ALTER TABLE sortie ADD COLUMN return_reason INTEGER; " +
                        "UPDATE sortie SET return_reason = (SELECT extra_info FROM sortie_detail WHERE id = sortie.id AND node = -1); " +
                        "DELETE FROM sortie_detail WHERE node = -1;";

                    rCommand.ExecuteNonQuery();
                }

            if (rpOldVersion < 5)
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "ALTER TABLE sortie ADD COLUMN map_hp INTEGER;";

                    rCommand.ExecuteNonQuery();
                }
        }

        protected override void Load()
        {
            if (InsertReturnReason(ReturnReason.Unexpected))
                ApiService.SubscribeOnce("api_port/port", r => SetReturnTime(r.Timestamp));
        }

        public override void Dispose()
        {
            KanColleGame.Current.ReturnedFromSortie -= OnReturnedFromSortie;

            base.Dispose();
        }

        void StartSortie(ApiInfo rpInfo)
        {
            r_ReturnReason = null;

            var rSortie = SortieInfo.Current;
            var rMap = rSortie.Map;

            using (var rTransaction = Connection.BeginTransaction())
            {
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "INSERT OR IGNORE INTO sortie_map(id, is_event_map) VALUES(@map_id, @is_event_map);" +
                        "INSERT INTO sortie(id, map, difficulty, map_hp) VALUES(@sortie_id, @map_id, @difficulty, @map_hp);";
                    rCommand.Parameters.AddWithValue("@map_id", rMap.ID);
                    rCommand.Parameters.AddWithValue("@sortie_id", rSortie.ID);
                    rCommand.Parameters.AddWithValue("@is_event_map", rMap.IsEventMap);
                    rCommand.Parameters.AddWithValue("@difficulty", rMap.Difficulty);
                    rCommand.Parameters.AddWithValue("@map_hp", rMap.HasGauge ? rMap.HP.Current : (int?)null);

                    if (rMap.HasGauge)
                    {
                        rCommand.CommandText += "INSERT OR IGNORE INTO sortie_map_hp(id, difficulty, hp) VALUES(@map_id, coalesce(@difficulty, 0), @map_max_hp);";

                        rCommand.Parameters.AddWithValue("@map_max_hp", rMap.HP.Maximum);
                    }

                    rCommand.ExecuteNonQuery();
                }

                rTransaction.Commit();
            }

            InsertExplorationRecord(rSortie);
        }
        void InsertExplorationRecord(SortieInfo rpSortie)
        {
            var rNode = rpSortie.Node;

            using (var rTransaction = Connection.BeginTransaction())
            {
                InsertNodeInfo(rpSortie.Map.ID, rNode);
                InsertRecord(rpSortie.ID, rNode.InternalID, (rNode.Event as IExtraInfo)?.GetExtraInfo());

                if (!rpSortie.Map.IsCleared && rNode.EventType == SortieEventType.EscortSuccess)
                    ProcessEscortSuccess(rpSortie);

                rTransaction.Commit();
            }

            if (rpSortie.Node.IsDeadEnd)
                r_ReturnReason = ReturnReason.DeadEnd;
        }
        void InsertNodeInfo(int rpMapID, SortieNodeInfo rpNode)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO sortie_node(map, id, type, subtype) VALUES(@map, @id, @type, @subtype);";
                rCommand.Parameters.AddWithValue("@map", rpMapID);
                rCommand.Parameters.AddWithValue("@id", rpNode.ID);
                rCommand.Parameters.AddWithValue("@type", (int)rpNode.EventType);
                rCommand.Parameters.AddWithValue("@subtype", rpNode.EventSubType);

                rCommand.ExecuteNonQuery();
            }
        }
        void InsertRecord(long rpSortieID, int rpNode, long? rpExtraInfo = null)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO sortie_detail(id, step, node, extra_info) VALUES(@id, (SELECT coalesce(max(step), 0) + 1 FROM sortie_detail WHERE id = @id), @node, @extra_info);";
                rCommand.Parameters.AddWithValue("@id", rpSortieID);
                rCommand.Parameters.AddWithValue("@node", rpNode);
                rCommand.Parameters.AddWithValue("@extra_info", rpExtraInfo);

                rCommand.ExecuteNonQuery();
            }
        }

        void RecordMapHP(ApiInfo rpInfo)
        {
            var rSortie = SortieInfo.Current;
            var rSortieMap = rSortie.Map;
            if (rSortieMap.HP == null || rSortie.Node.EventType != SortieEventType.BossBattle)
                return;

            SetSortieMapHP(rSortie.ID, rSortieMap.HP.Current);
        }
        void ProcessEscortSuccess(SortieInfo rpSortie) => SetSortieMapHP(rpSortie.ID, rpSortie.Map.HP.Current);
        void SetSortieMapHP(long rpID, int rpMapHP)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE sortie SET map_hp = @map_hp WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpID);
                rCommand.Parameters.AddWithValue("@map_hp", rpMapHP > 0 ? (int?)rpMapHP : null);

                rCommand.ExecuteNonQuery();
            }
        }

        void OnReturnedFromSortie(SortieInfo rpSortie)
        {
            if (!r_ReturnReason.HasValue)
            {
                IEnumerable<Ship> rShips = rpSortie.Fleet.Ships;
                if (rpSortie.EscortFleet != null)
                    rShips = rShips.Concat(rpSortie.EscortFleet.Ships);

                r_ReturnReason = rShips.Any(rpShip => (rpShip.State & ShipState.HeavilyDamaged) != 0) ? ReturnReason.RetreatWithHeavilyDamagedShip : ReturnReason.Retreat;
            }

            using (var rTransaction = Connection.BeginTransaction())
            {
                InsertReturnReason(r_ReturnReason.Value);
                SetReturnTime(rpSortie.ReturnTime);

                rTransaction.Commit();
            }
        }

        bool InsertReturnReason(ReturnReason rpReason)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE sortie SET return_reason = @return_reason WHERE id = (SELECT MAX(id) FROM sortie) AND return_reason IS NULL;";
                rCommand.Parameters.AddWithValue("@return_reason", (int)rpReason);

                rCommand.ExecuteNonQuery();
            }

            return Connection.Changes == 1;
        }

        void SetReturnTime(long rpTimestamp)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "UPDATE sortie SET return_time = @return_time WHERE id = (SELECT MAX(id) FROM sortie);";
                rCommand.Parameters.AddWithValue("@return_time", rpTimestamp);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
