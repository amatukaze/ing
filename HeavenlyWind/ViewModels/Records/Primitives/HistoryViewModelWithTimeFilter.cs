using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives
{
    abstract class HistoryViewModelWithTimeFilter<T> : HistoryViewModel<T> where T : ModelBase, IRecordID
    {
        public IList<TimeSpanViewModel> TimeSpans { get; }

        public CustomTimeSpanViewModel CustomTimeSpan { get; }

        TimeSpanViewModel r_SelectedTimeSpan;
        public TimeSpanViewModel SelectedTimeSpan
        {
            get { return r_SelectedTimeSpan; }
            set
            {
                if (r_SelectedTimeSpan != value)
                {
                    r_SelectedTimeSpan = value;
                    OnPropertyChanged(nameof(SelectedTimeSpan));

                    LoadRecords();
                }
            }
        }

        PropertyChangedEventListener r_CustomTimeSpanPCEL;

        protected HistoryViewModelWithTimeFilter()
        {
            TimeSpans = new TimeSpanViewModel[]
            {
                new GeneralTimeSpanViewModel(TimeSpanType.All),
                r_SelectedTimeSpan = new GeneralTimeSpanViewModel(TimeSpanType.Recent24Hours),
                new GeneralTimeSpanViewModel(TimeSpanType.Recent3Days),
                new GeneralTimeSpanViewModel(TimeSpanType.Recent7Days),

                CustomTimeSpan = new CustomTimeSpanViewModel(),
            };

            r_CustomTimeSpanPCEL = new PropertyChangedEventListener(CustomTimeSpan);
            r_CustomTimeSpanPCEL.Add(nameof(CustomTimeSpan.SelectedDateStart), (_, __) => LoadRecords());
            r_CustomTimeSpanPCEL.Add(nameof(CustomTimeSpan.SelectedDateEnd), (_, __) => LoadRecords());
        }

        protected override void SetCommandText(SQLiteCommand rpCommand) =>
            rpCommand.CommandText = string.Format(LoadCommandText, r_SelectedTimeSpan.TimeSpanStart, r_SelectedTimeSpan.TimeSpanEnd);

        protected bool IsInTimeSpan(long rpRowID) => rpRowID >= long.Parse(CustomTimeSpan.TimeSpanStart) && rpRowID < long.Parse(CustomTimeSpan.TimeSpanEnd);

        protected override void DisposeManagedResources()
        {
            r_CustomTimeSpanPCEL.Dispose();

            base.DisposeManagedResources();
        }
    }
}
