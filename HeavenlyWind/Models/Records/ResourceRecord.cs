using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class ResourceRecord : ModelBase
    {
        public string Time { get; }

        public int Fuel { get; }
        public int FuelDifference { get; }

        public int Bullet { get; }
        public int BulletDifference { get; }

        public int Steel { get; }
        public int SteelDifference { get; }

        public int Bauxite { get; }
        public int BauxiteDifference { get; }

        public int InstantConstruction { get; }
        public int InstantConstructionDifference { get; }

        public int Bucket { get; }
        public int BucketDifference { get; }

        public int DevelopmentMaterial { get; }
        public int DevelopmentMaterialDifference { get; }

        public int ImprovementMaterial { get; }
        public int ImprovementMaterialDifference { get; }

        public ResourceRecord(SQLiteDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(rpReader.GetInt64("time")).LocalDateTime.ToString();

            Fuel = rpReader.GetInt32("fuel");
            FuelDifference = rpReader.GetInt32("fuel_diff");

            Bullet = rpReader.GetInt32("bullet");
            BulletDifference = rpReader.GetInt32("bullet_diff");

            Steel = rpReader.GetInt32("steel");
            SteelDifference = rpReader.GetInt32("steel_diff");

            Bauxite = rpReader.GetInt32("bauxite");
            BauxiteDifference = rpReader.GetInt32("bauxite_diff");

            InstantConstruction = rpReader.GetInt32("instant_construction");
            InstantConstructionDifference = rpReader.GetInt32("instant_construction_diff");

            Bucket = rpReader.GetInt32("bucket");
            BucketDifference = rpReader.GetInt32("bucket_diff");

            DevelopmentMaterial = rpReader.GetInt32("development_material");
            DevelopmentMaterialDifference = rpReader.GetInt32("development_material_diff");

            ImprovementMaterial = rpReader.GetInt32("improvement_material");
            ImprovementMaterialDifference = rpReader.GetInt32("improvement_material_diff");
        }
    }
}
