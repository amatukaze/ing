using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ConstructionHistoryViewModel : WindowViewModel, IDisposable
    {
        ObservableRangeCollection<ConstructionRecord.RecordItem> r_Records;
        public ReadOnlyObservableCollection<ConstructionRecord.RecordItem> Records { get; }

        IDisposable r_NewRecordSubscription;

        public ConstructionHistoryViewModel()
        {
            Title = StringResources.Instance.Main.Window_ConstructionHistory;

            r_Records = new ObservableRangeCollection<ConstructionRecord.RecordItem>();
            Records = new ReadOnlyObservableCollection<ConstructionRecord.RecordItem>(r_Records);

            r_NewRecordSubscription = ConstructionRecord.NewRecord.ObserveOnDispatcher().Subscribe(r => r_Records.Insert(0, r));
        }

        public async void LoadRecords() => r_Records.AddRange(await RecordService.Instance.Construction.LoadRecordsAsync());

        public void Dispose() => r_NewRecordSubscription?.Dispose();
    }
}
