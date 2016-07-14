using Sakuno.KanColle.Amatsukaze.Models.Records;
using System.Data.SQLite;
using System.IO;

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

        protected override void ExportAsCsvFileCore(StreamWriter rpWriter)
        {
            var rSR = StringResources.Instance.Main;
            var rDiff = "_" + rSR.ResourceHistory_Diff;

            rpWriter.Write(rSR.Record_Time);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Fuel);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Fuel);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Bullet);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Bullet);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Steel);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Steel);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Bauxite);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Bauxite);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_InstantConstruction);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_InstantConstruction);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Bucket);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_Bucket);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_DevelopmentMaterial);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_DevelopmentMaterial);
            rpWriter.Write(rDiff);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_ImprovementMaterial);
            rpWriter.Write(',');
            rpWriter.Write(rSR.Material_ImprovementMaterial);
            rpWriter.WriteLine(rDiff);

            foreach (var rRecord in Records)
            {
                rpWriter.Write(rRecord.Time);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Fuel);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.FuelDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Bullet);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.BulletDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Steel);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.SteelDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Bauxite);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.BauxiteDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.InstantConstruction);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.InstantConstructionDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.Bucket);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.BucketDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.DevelopmentMaterial);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.DevelopmentMaterialDifference);
                rpWriter.Write(',');
                rpWriter.Write(rRecord.ImprovementMaterial);
                rpWriter.Write(',');
                rpWriter.WriteLine(rRecord.ImprovementMaterialDifference);
            }
        }
    }
}
