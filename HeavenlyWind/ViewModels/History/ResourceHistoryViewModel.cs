using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ResourceHistoryViewModel : ModelBase
    {
        public IList<ResourceRecord> Records { get; private set; }

        public async void LoadRecords()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = @"SELECT x.time,
    x.fuel, coalesce(x.fuel - y.fuel, 0) AS fuel_diff,
    x.bullet, coalesce(x.bullet - y.bullet, 0) AS bullet_diff,
    x.steel, coalesce(x.steel - y.steel, 0) AS steel_diff,
    x.bauxite, coalesce(x.bauxite - y.bauxite, 0) AS bauxite_diff,
    x.instant_construction, coalesce(x.instant_construction - y.instant_construction, 0) AS instant_construction_diff,
    x.bucket, coalesce(x.bucket - y.bucket, 0) AS bucket_diff,
    x.development_material, coalesce(x.development_material - y.development_material, 0) AS development_material_diff,
    x.improvement_material, coalesce(x.improvement_material - y.improvement_material, 0) AS improvement_material_diff
FROM resources x
JOIN resources y ON y.time = (SELECT max(time) FROM resources z WHERE z.time < x.time)
ORDER BY x.time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                {
                    var rRecords = new List<ResourceRecord>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                        rRecords.Add(new ResourceRecord(rReader));

                    Records = rRecords;
                }
            }

            OnPropertyChanged(nameof(Records));
        }
    }
}
