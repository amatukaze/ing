using Sakuno.KanColle.Amatsukaze.Models.Records;
using Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records
{
    class ResourceHistoryViewModel : HistoryViewModel<ResourceRecord>
    {
        public IList<ResourceHistoryTypeKey> Types { get; }

        ResourceHistoryTypeKey r_Type;
        public ResourceHistoryTypeKey SelectedType
        {
            get { return r_Type; }
            set
            {
                if (r_Type != value)
                {
                    r_Type = value;
                    OnPropertyChanged(nameof(SelectedType));

                    r_Type.Update();

                    LoadRecords();
                }
            }
        }

        protected override string LoadCommandText => $"SELECT * FROM resources {r_Type.Clause};";

        public ResourceHistoryViewModel()
        {
            Types = Enumerable.Range(0, 4).Select(r => new ResourceHistoryTypeKey((ResourceHistoryType)r)).ToArray();
            r_Type = Types[1];
            r_Type.Update();
        }

        protected override ResourceRecord CreateRecordFromReader(SQLiteDataReader rpReader)
        {
            var rResult = r_Type.RecordConstructor(rpReader);

            if (LastInsertedRecord != null)
                rResult.Previous = LastInsertedRecord;

            return rResult;
        }

        protected override bool TableFilter(string rpTable) => rpTable == "main.resources";

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            rpCommand.CommandText = "SELECT * FROM resources WHERE time = @time;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
        protected override bool BeforeNewRecordInserting(SQLiteDataReader rpReader)
        {
            if (r_Type.Type == ResourceHistoryType.Detail)
                return true;

            var rTime = rpReader.GetInt64("time");

            if (rTime >= r_Type.Maximum)
            {
                r_Type.Update();

                return true;
            }

            LastInsertedRecord.ID = rTime;
            LastInsertedRecord.Fuel = rpReader.GetInt32("fuel");
            LastInsertedRecord.Bullet = rpReader.GetInt32("bullet");
            LastInsertedRecord.Steel = rpReader.GetInt32("steel");
            LastInsertedRecord.Bauxite = rpReader.GetInt32("bauxite");
            LastInsertedRecord.InstantConstruction = rpReader.GetInt32("instant_construction");
            LastInsertedRecord.Bucket = rpReader.GetInt32("bucket");
            LastInsertedRecord.DevelopmentMaterial = rpReader.GetInt32("development_material");
            LastInsertedRecord.ImprovementMaterial = rpReader.GetInt32("improvement_material");

            LastInsertedRecord.Update();

            return false;
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
