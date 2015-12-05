using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class DevelopmentHistoryViewModel : WindowViewModel, IDisposable
    {
        ObservableRangeCollection<DevelopmentRecord.RecordItem> r_Records;
        public ReadOnlyObservableCollection<DevelopmentRecord.RecordItem> Records { get; }

        IDisposable r_NewRecordSubscription;

        public DevelopmentHistoryViewModel()
        {
            Title = StringResources.Instance.Main.Window_DevelopmentHistory;

            r_Records = new ObservableRangeCollection<DevelopmentRecord.RecordItem>();
            Records = new ReadOnlyObservableCollection<DevelopmentRecord.RecordItem>(r_Records);

            r_NewRecordSubscription = DevelopmentRecord.NewRecord.ObserveOnDispatcher().Subscribe(r => r_Records.Insert(0, r));
        }

        public async void LoadRecords() => r_Records.AddRange(await RecordService.Instance.Development.LoadRecordsAsync());

        public void Dispose() => r_NewRecordSubscription?.Dispose();
    }
}
