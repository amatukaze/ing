using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class DevelopmentHistoryViewModel : HistoryViewModelBase<DevelopmentRecord>
    {
        protected override string LoadCommandText => "SELECT * FROM development ORDER BY time DESC;";

        public FilterKeyCollection<EquipmentInfo> Equipment { get; } = new FilterKeyCollection<EquipmentInfo>(EquipmentInfo.Dummy);
        public FilterKeyCollection<ShipInfo> SecretaryShips { get; } = new FilterKeyCollection<ShipInfo>(ShipInfo.Dummy);

        bool r_SuccessfulRecordsOnly;
        public bool SuccessfulRecordsOnly
        {
            get { return r_SuccessfulRecordsOnly; }
            set
            {
                if (r_SuccessfulRecordsOnly != value)
                {
                    r_SuccessfulRecordsOnly = value;
                    OnPropertyChanged(nameof(SuccessfulRecordsOnly));

                    Refresh();
                }
            }
        }

        EquipmentInfo r_Equipment = EquipmentInfo.Dummy;
        public EquipmentInfo SelectedEquipment
        {
            get { return r_Equipment; }
            set
            {
                if (r_Equipment != value)
                {
                    r_Equipment = value;
                    OnPropertyChanged(nameof(SelectedEquipment));

                    Refresh();
                }
            }
        }

        ShipInfo r_SecretaryShip = ShipInfo.Dummy;
        public ShipInfo SelectedSecretaryShip
        {
            get { return r_SecretaryShip; }
            set
            {
                if (r_SecretaryShip != value)
                {
                    r_SecretaryShip = value;
                    OnPropertyChanged(nameof(SelectedSecretaryShip));

                    Refresh();
                }
            }
        }

        public override void OnInitialized()
        {
            var rMasterInfo = KanColleGame.Current.MasterInfo;

            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = "SELECT DISTINCT equipment FROM development WHERE equipment IS NOT NULL ORDER BY equipment;";
                using (var rReader = rCommand.ExecuteReader())
                {
                    var rEquipment = new List<EquipmentInfo>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rEquipment.Add(rMasterInfo.Equipment[rReader.GetInt32(0)]);

                    Equipment.AddRange(rEquipment);
                }

                rCommand.CommandText = "SELECT DISTINCT flagship FROM development WHERE equipment IS NOT NULL ORDER BY flagship;";
                using (var rReader = rCommand.ExecuteReader())
                {
                    var rSecretaryShips = new List<ShipInfo>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rSecretaryShips.Add(rMasterInfo.Ships[rReader.GetInt32(0)]);

                    SecretaryShips.AddRange(rSecretaryShips);
                }
            }
        }

        public override bool Filter(DevelopmentRecord rpItem) =>
            (!r_SuccessfulRecordsOnly || rpItem.Equipment != null) &&
            (r_Equipment == EquipmentInfo.Dummy || rpItem.Equipment == r_Equipment) &&
            (r_SecretaryShip == ShipInfo.Dummy || rpItem.SecretaryShip == r_SecretaryShip);

        protected override DevelopmentRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new DevelopmentRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.development";

        protected override void OnRecordInsert(string rpTable, long rpRowID)
        {
            base.OnRecordInsert(rpTable, rpRowID);

            Equipment.AddIfAbsent(LastInsertedRecord.Equipment);
            SecretaryShips.AddIfAbsent(LastInsertedRecord.SecretaryShip);
        }

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = "SELECT * FROM development WHERE time = @time LIMIT 1;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
