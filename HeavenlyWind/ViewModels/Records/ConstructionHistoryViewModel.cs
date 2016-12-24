using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records
{
    class ConstructionHistoryViewModel : HistoryViewModelWithTimeFilter<ConstructionRecord>
    {
        protected override string LoadCommandText => "SELECT * FROM construction WHERE time >= {0} AND time < {1};";

        public FilterKeyCollection<ShipInfo> Ships { get; } = new FilterKeyCollection<ShipInfo>(ShipInfo.Dummy, (x, y) => x.ID - y.ID);

        bool r_LSCOnly;
        public bool LSCOnly
        {
            get { return r_LSCOnly; }
            set
            {
                if (r_LSCOnly != value)
                {
                    r_LSCOnly = value;
                    OnPropertyChanged(nameof(LSCOnly));

                    Refresh();
                }
            }
        }

        ShipInfo r_Ship = ShipInfo.Dummy;
        public ShipInfo SelectedShip
        {
            get { return r_Ship; }
            set
            {
                if (r_Ship != value)
                {
                    r_Ship = value;
                    OnPropertyChanged(nameof(SelectedShip));

                    Refresh();
                }
            }
        }

        public override void OnInitialized()
        {
            var rMasterInfo = KanColleGame.Current.MasterInfo;

            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = "SELECT DISTINCT ship FROM construction ORDER BY ship;";
                using (var rReader = rCommand.ExecuteReader())
                {
                    var rShips = new List<ShipInfo>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rShips.Add(rMasterInfo.Ships[rReader.GetInt32(0)]);

                    Ships.AddRange(rShips);
                }
            }
        }

        public override bool Filter(ConstructionRecord rpItem) =>
            (!r_LSCOnly || rpItem.IsLargeShipConstruction) &&
            (r_Ship == ShipInfo.Dummy || rpItem.Ship == r_Ship);

        protected override ConstructionRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new ConstructionRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.construction";

        protected override void OnRecordInsert(string rpTable, long rpRowID)
        {
            base.OnRecordInsert(rpTable, rpRowID);

            Ships.AddIfAbsent(LastInsertedRecord.Ship);
        }

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            if (SelectedTimeSpan.Type == TimeSpanType.Custom && !IsInTimeSpan(rpRowID))
                return;

            rpCommand.CommandText = "SELECT * FROM construction WHERE time = @time LIMIT 1;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
