using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class SortieRecord : RecordBase
    {
        const int RETURN_CELL_ID = -1;
        enum ReturnReason { DeadEnd, Retreat, RetreatWithHeavilyDamagedShip, Unexpected }

        public override string GroupName => "sortie";

        long? r_CurrentSortieID;
        bool r_IsDeadEnd;

        IDisposable r_CellSubscription;

        internal SortieRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_req_map/start", StartSortie));

            DisposableObjects.Add(SessionService.Instance.Subscribe("api_start2", _ => ProcessReturn(ReturnReason.Unexpected)));
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_port/port", _ =>
            {
                r_CellSubscription?.Dispose();

                ReturnReason rType;

                if (r_IsDeadEnd)
                    rType = ReturnReason.DeadEnd;
                else
                    rType = ReturnReason.Retreat;

                ProcessReturn(rType);
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS sortie_map(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "is_event_map BOOLEAN NOT NULL)";

                rCommand.ExecuteNonQuery();
            }
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS sortie(" +
                    "id INTEGER PRIMARY KEY NOT NULL, " +
                    "map INTEGER NOT NULL REFERENCES sortie_map(id))";

                rCommand.ExecuteNonQuery();
            }
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS sortie_cell(" +
                    "map INTEGER NOT NULL REFERENCES sortie_map(id), " +
                    "id INTEGER NOT NULL, " +
                    "type INTEGER NOT NULL, " +
                    "subtype INTEGER NOT NULL, " +
                    "CONSTRAINT [] PRIMARY KEY(map, id)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS sortie_detail(" +
                    "id INTEGER NOT NULL REFERENCES sortie(id), " +
                    "cell INTEGER NOT NULL, " +
                    "extra_info INTEGER, " +
                    "CONSTRAINT [] PRIMARY KEY(id, cell)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }

        protected override void Load() => ProcessUnexpectedReturn();

        void StartSortie(ApiData rpData)
        {
            var rSortie = KanColleGame.Current.Sortie;
            var rMap = rSortie.Map;
            r_CurrentSortieID = rSortie.ID;

            using (var rTransaction = Connection.BeginTransaction())
            {
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "INSERT OR IGNORE INTO sortie_map(id, is_event_map) VALUES (@id, @is_event_map);";
                    rCommand.Parameters.AddWithValue("@id", rMap.ID);
                    rCommand.Parameters.AddWithValue("@is_event_map", rMap.IsEventMap);

                    rCommand.ExecuteNonQuery();
                }
                using (var rCommand = Connection.CreateCommand())
                {
                    rCommand.CommandText = "INSERT INTO sortie(id, map) VALUES (@id, @map);";
                    rCommand.Parameters.AddWithValue("@id", rSortie.ID);
                    rCommand.Parameters.AddWithValue("@map", rMap.ID);

                    rCommand.ExecuteNonQuery();
                }

                rTransaction.Commit();
            }

            r_CellSubscription = Observable.FromEventPattern<PropertyChangedEventArgs>(rSortie, nameof(rSortie.PropertyChanged))
                .Where(r => r.EventArgs.PropertyName == nameof(rSortie.Cell))
                .Subscribe(r_ => InsertExplorationRecord(KanColleGame.Current.Sortie));
        }

        void InsertExplorationRecord(SortieInfo rpSortie)
        {
            using (var rTransaction = Connection.BeginTransaction())
            {
                InsertCellInfo(rpSortie.Map.ID, rpSortie.Cell);
                InsertRecord(rpSortie.ID, rpSortie.Cell.InternalID);

                rTransaction.Commit();
            }

            r_IsDeadEnd = rpSortie.Cell.IsDeadEnd;
        }
        void InsertCellInfo(int rpMapID, SortieCellInfo rpCell)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO sortie_cell(map, id, type, subtype) VALUES (@map, @id, @type, @subtype);";
                rCommand.Parameters.AddWithValue("@map", rpMapID);
                rCommand.Parameters.AddWithValue("@id", rpCell.ID);
                rCommand.Parameters.AddWithValue("@type", (int)rpCell.EventType);
                rCommand.Parameters.AddWithValue("@subtype", rpCell.EventSubType);

                rCommand.ExecuteNonQuery();
            }
        }
        void InsertRecord(long rpSortieID, int rpCell, int? rpExtraInfo = null)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO sortie_detail(id, cell, extra_info) VALUES (@id, @cell, @extra_info);";
                rCommand.Parameters.AddWithValue("@id", rpSortieID);
                rCommand.Parameters.AddWithValue("@cell", rpCell);
                rCommand.Parameters.AddWithValue("@extra_info", rpExtraInfo);

                rCommand.ExecuteNonQuery();
            }
        }

        void InsertReturnRecord(long rpSortieID, ReturnReason rpType) => InsertRecord(rpSortieID, RETURN_CELL_ID, (int)rpType);
        void ProcessUnexpectedReturn()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR IGNORE INTO sortie_detail(id, cell, extra_info) VALUES((SELECT MAX(id) FROM sortie), -1, @return_reason);";
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
