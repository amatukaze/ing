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
        const int RETURN_NODE_ID = -1;
        enum ReturnReason { DeadEnd, Retreat, RetreatWithHeavilyDamagedShip, Unexpected }

        public override string GroupName => "sortie";
        public override int Version => 3;

        long? r_CurrentSortieID;
        bool r_IsDeadEnd;

        internal SortieRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_map/start", StartSortie));
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_map/next", _ => InsertExplorationRecord(SortieInfo.Current)));

            DisposableObjects.Add(SessionService.Instance.Subscribe("api_start2", _ => ProcessReturn(ReturnReason.Unexpected)));
            DisposableObjects.Add(Observable.FromEvent<SortieInfo>(r => KanColleGame.Current.ReturnedFromSortie += r, r => KanColleGame.Current.ReturnedFromSortie -= r).Subscribe(r =>
            {
                ReturnReason rType;

                if (r_IsDeadEnd)
                    rType = ReturnReason.DeadEnd;
                else
                {
                    IEnumerable<Ship> rShips = r.Fleet.Ships;
                    if (r.EscortFleet != null)
                        rShips = rShips.Concat(r.EscortFleet.Ships);

                    rType = rShips.Any(rpShip => rpShip.State.HasFlag(ShipState.HeavilyDamaged)) ? ReturnReason.RetreatWithHeavilyDamagedShip : ReturnReason.Retreat;
                }

                ProcessReturn(rType);
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS sortie_map(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "is_event_map BOOLEAN NOT NULL);" +

                "CREATE TABLE IF NOT EXISTS sortie(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "map INTEGER NOT NULL REFERENCES sortie_map(id), " +
                    "difficulty INTEGER);" +

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

        protected override void UpgradeFromOldVersion(int rpOldVersion)
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
        }

        protected override void Load() => InsertReturnReason(ReturnReason.Unexpected);

        void StartSortie(ApiData rpData)
        {
            var rSortie = SortieInfo.Current;
            var rMap = rSortie.Map;
            r_CurrentSortieID = rSortie.ID;

            using (var rTransaction = Connection.BeginTransaction())
            {
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText =
                        "INSERT OR IGNORE INTO sortie_map(id, is_event_map) VALUES(@map_id, @is_event_map);" +
                        "INSERT INTO sortie(id, map, difficulty) VALUES(@sortie_id, @map_id, @difficulty);";
                    rCommand.Parameters.AddWithValue("@map_id", rMap.ID);
                    rCommand.Parameters.AddWithValue("@sortie_id", rSortie.ID);
                    rCommand.Parameters.AddWithValue("@is_event_map", rMap.IsEventMap);
                    rCommand.Parameters.AddWithValue("@difficulty", rMap.Difficulty);

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

                rTransaction.Commit();
            }

            r_IsDeadEnd = rpSortie.Node.IsDeadEnd;
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

        void InsertReturnReason(ReturnReason rpReason)
        {
            long rID;
            int rStep;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT id, step, node FROM sortie_detail WHERE id = (SELECT MAX(id) FROM sortie) ORDER BY step DESC LIMIT 1;";
                using (var rReader = rCommand.ExecuteReader())
                {
                    if (!rReader.Read() || Convert.ToInt32(rReader["node"]) == RETURN_NODE_ID)
                        return;

                    rID = Convert.ToInt64(rReader["id"]);
                    rStep = Convert.ToInt32(rReader["step"]) + 1;
                }
            }
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO sortie_detail(id, step, node, extra_info) VALUES(@id, @step, -1, @return_reason);";
                rCommand.Parameters.AddWithValue("@id", rID);
                rCommand.Parameters.AddWithValue("@step", rStep);
                rCommand.Parameters.AddWithValue("@return_reason", (int)rpReason);

                rCommand.ExecuteNonQuery();
            }
        }
        void ProcessReturn(ReturnReason rpType)
        {
            if (r_CurrentSortieID.HasValue)
            {
                InsertReturnReason(rpType);

                r_CurrentSortieID = null;
            }
        }
    }
}
