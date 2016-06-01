using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ResourceHistoryViewModel : HistoryViewModelBase<ResourceRecord>
    {
        protected override string LoadCommandText => @"SELECT x.time,
    x.fuel, coalesce(x.fuel - y.fuel, 0) AS fuel_diff,
    x.bullet, coalesce(x.bullet - y.bullet, 0) AS bullet_diff,
    x.steel, coalesce(x.steel - y.steel, 0) AS steel_diff,
    x.bauxite, coalesce(x.bauxite - y.bauxite, 0) AS bauxite_diff,
    x.instant_construction, coalesce(x.instant_construction - y.instant_construction, 0) AS instant_construction_diff,
    x.bucket, coalesce(x.bucket - y.bucket, 0) AS bucket_diff,
    x.development_material, coalesce(x.development_material - y.development_material, 0) AS development_material_diff,
    x.improvement_material, coalesce(x.improvement_material - y.improvement_material, 0) AS improvement_material_diff
FROM resources x
LEFT JOIN resources y ON y.time = (SELECT max(time) FROM resources z WHERE z.time < x.time)
ORDER BY x.time DESC;";

        protected override ResourceRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new ResourceRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.resources";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = @"SELECT x.time,
    x.fuel, coalesce(x.fuel - y.fuel, 0) AS fuel_diff,
    x.bullet, coalesce(x.bullet - y.bullet, 0) AS bullet_diff,
    x.steel, coalesce(x.steel - y.steel, 0) AS steel_diff,
    x.bauxite, coalesce(x.bauxite - y.bauxite, 0) AS bauxite_diff,
    x.instant_construction, coalesce(x.instant_construction - y.instant_construction, 0) AS instant_construction_diff,
    x.bucket, coalesce(x.bucket - y.bucket, 0) AS bucket_diff,
    x.development_material, coalesce(x.development_material - y.development_material, 0) AS development_material_diff,
    x.improvement_material, coalesce(x.improvement_material - y.improvement_material, 0) AS improvement_material_diff
FROM resources x
LEFT JOIN resources y ON y.time = (SELECT max(time) FROM resources z WHERE z.time < @time)
WHERE x.time = @time;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
