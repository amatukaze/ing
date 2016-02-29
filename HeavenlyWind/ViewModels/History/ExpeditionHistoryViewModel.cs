using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ExpeditionHistoryViewModel : WindowViewModel
    {
        public IList<ExpeditionRecord.RecordItem> Records { get; private set; }

        public async void LoadRecords()
        {
            Records = await RecordService.Instance.Expedition.LoadRecordsAsync();

            OnPropertyChanged(nameof(Records));
        }
    }
}
