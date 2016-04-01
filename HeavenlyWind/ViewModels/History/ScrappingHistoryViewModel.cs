using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ScrappingHistoryViewModel : WindowViewModel
    {
        public IList<FateRecord.RecordItem> Records { get; private set; }

        public async void LoadRecords()
        {
            Records = await RecordService.Instance.Fate.LoadRecordsAsync();
            OnPropertyChanged(nameof(Records));
        }
    }
}
