using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ExpeditionHistoryViewModel : ModelBase
    {
        public IList<ExpeditionRecord> Records { get; private set; }

        public async void LoadRecords()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM expedition ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rRecords = new List<ExpeditionRecord>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rRecords.Add(new ExpeditionRecord(rReader));

                    Records = rRecords;
                }
            }

            OnPropertyChanged(nameof(Records));
        }
    }
}
