using System;
using System.Data.Common;

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

        public ResourceRecord(DbDataReader rpReader)
        {
            Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["time"])).LocalDateTime.ToString();

            Fuel = Convert.ToInt32(rpReader["fuel"]);
            FuelDifference = Convert.ToInt32(rpReader["fuel_diff"]);

            Bullet = Convert.ToInt32(rpReader["bullet"]);
            BulletDifference = Convert.ToInt32(rpReader["bullet_diff"]);

            Steel = Convert.ToInt32(rpReader["steel"]);
            SteelDifference = Convert.ToInt32(rpReader["steel_diff"]);

            Bauxite = Convert.ToInt32(rpReader["bauxite"]);
            BauxiteDifference = Convert.ToInt32(rpReader["bauxite_diff"]);

            InstantConstruction = Convert.ToInt32(rpReader["instant_construction"]);
            InstantConstructionDifference = Convert.ToInt32(rpReader["instant_construction_diff"]);

            Bucket = Convert.ToInt32(rpReader["bucket"]);
            BucketDifference = Convert.ToInt32(rpReader["bucket_diff"]);

            DevelopmentMaterial = Convert.ToInt32(rpReader["development_material"]);
            DevelopmentMaterialDifference = Convert.ToInt32(rpReader["development_material_diff"]);

            ImprovementMaterial = Convert.ToInt32(rpReader["improvement_material"]);
            ImprovementMaterialDifference = Convert.ToInt32(rpReader["improvement_material_diff"]);
        }
    }
}
