using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class SortieHistoryViewModel : WindowViewModel
    {
        public IList<SortieRecord.RecordItem> Data { get; private set; }

        public async void LoadRecords()
        {
            Data = await RecordService.Instance.Sortie.LoadRecordsAsync();

            OnPropertyChanged(nameof(Data));
        }
    }
}
