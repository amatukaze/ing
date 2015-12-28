using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class SortieRecord : RecordBase
    {
        const int RETURN_NODE_ID = -1;
        enum ReturnReason { DeadEnd, Retreat, RetreatWithHeavilyDamagedShip, Unexpected }

        public override string GroupName => "sortie";
        public override int Version => 2;

        long? r_CurrentSortieID;
        int r_Step;
        bool r_IsDeadEnd;

        IDisposable r_NodeSubscription;

        internal SortieRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_map/start", StartSortie));

            DisposableObjects.Add(SessionService.Instance.Subscribe("api_start2", _ => ProcessReturn(ReturnReason.Unexpected)));
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_port/port", _ =>
            {
                if (r_NodeSubscription != null)
                {
                    r_NodeSubscription.Dispose();

                    ReturnReason rType;

                    if (r_IsDeadEnd)
                        rType = ReturnReason.DeadEnd;
                    else
                    {
                        var rSortie = KanColleGame.Current.OldSortie;

                        IEnumerable<Ship> rShips = rSortie.Fleet.Ships;
                        if (rSortie.EscortFleet != null)
                            rShips = rShips.Concat(rSortie.EscortFleet.Ships);

                        rType = rShips.Any(r => r.State.HasFlag(ShipState.HeavilyDamaged)) ? ReturnReason.RetreatWithHeavilyDamagedShip : ReturnReason.Retreat;
                    }

                    ProcessReturn(rType);
                }
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
                    "map INTEGER NOT NULL REFERENCES sortie_map(id));" +

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
        }

        protected override void Load() => ProcessUnexpectedReturn();

        void StartSortie(ApiData rpData)
        {
            var rSortie = KanColleGame.Current.Sortie;
            var rMap = rSortie.Map;
            r_CurrentSortieID = rSortie.ID;
            r_Step = 1;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "BEGIN TRANSACTION; " +
                    "INSERT OR IGNORE INTO sortie_map(id, is_event_map) VALUES(@map_id, @is_event_map);" +
                    "INSERT INTO sortie(id, map) VALUES(@sortie_id, @map_id);" +
                    "COMMIT;";
                rCommand.Parameters.AddWithValue("@map_id", rMap.ID);
                rCommand.Parameters.AddWithValue("@sortie_id", rSortie.ID);
                rCommand.Parameters.AddWithValue("@is_event_map", rMap.IsEventMap);

                rCommand.ExecuteNonQuery();
            }

            r_NodeSubscription = Observable.FromEventPattern<PropertyChangedEventArgs>(rSortie, nameof(rSortie.PropertyChanged))
                .Where(r => r.EventArgs.PropertyName == nameof(rSortie.Cell))
                .Subscribe(r_ => InsertExplorationRecord(KanColleGame.Current.Sortie));
        }

        void InsertExplorationRecord(SortieInfo rpSortie)
        {
            var rNode = rpSortie.Cell;

            using (var rTransaction = Connection.BeginTransaction())
            {
                InsertCellInfo(rpSortie.Map.ID, rNode);
                InsertRecord(rpSortie.ID, rNode.InternalID, (rNode.Event as BattleEvent)?.Battle.ID);

                rTransaction.Commit();
            }

            r_IsDeadEnd = rpSortie.Cell.IsDeadEnd;
        }
        void InsertCellInfo(int rpMapID, SortieCellInfo rpNode)
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
                rCommand.CommandText = "INSERT INTO sortie_detail(id, step, node, extra_info) VALUES(@id, @step, @node, @extra_info);";
                rCommand.Parameters.AddWithValue("@id", rpSortieID);
                rCommand.Parameters.AddWithValue("@step", r_Step++);
                rCommand.Parameters.AddWithValue("@node", rpNode);
                rCommand.Parameters.AddWithValue("@extra_info", rpExtraInfo);

                rCommand.ExecuteNonQuery();
            }
        }

        void InsertReturnRecord(long rpSortieID, ReturnReason rpType) => InsertRecord(rpSortieID, RETURN_NODE_ID, (int)rpType);
        void ProcessUnexpectedReturn()
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
                rCommand.Parameters.AddWithValue("@return_reason", (int)ReturnReason.Unexpected);

                rCommand.ExecuteNonQuery();
            }
        }
        void ProcessReturn(ReturnReason rpType)
        {
            if (r_CurrentSortieID.HasValue)
            {
                InsertReturnRecord(r_CurrentSortieID.Value, rpType);

                r_CurrentSortieID = null;
            }
        }

    }
}
