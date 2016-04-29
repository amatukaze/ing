using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ScrappingHistoryViewModel : ModelBase
    {
        public IList<FateRecord> Records { get; private set; }

        public async void LoadRecords()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = @"SELECT id, ship AS master_id, level, 0 AS proficiency, time, fate, 0 AS is_equipment FROM ship_fate
UNION
SELECT id, equipment AS master_id, level, proficiency, time, fate, 1 AS is_equipment FROM equipment_fate
ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rRecords = new List<FateRecord>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rRecords.Add(new FateRecord(rReader));

                    Records = rRecords;
                }
            }
            OnPropertyChanged(nameof(Records));
        }
    }
}
