using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class ResourceRecord : ModelBase, IRecordID
    {
        public long ID { get; set; }
        public virtual string Time => DateTimeUtil.FromUnixTime(ID).LocalDateTime.ToString();

        public ResourceRecord Previous { get; set; }

        public int Fuel { get; set; }
        public int FuelDifference => Previous != null ? Fuel - Previous.Fuel : 0;

        public int Bullet { get; set; }
        public int BulletDifference => Previous != null ? Bullet - Previous.Bullet : 0;

        public int Steel { get; set; }
        public int SteelDifference => Previous != null ? Steel - Previous.Steel : 0;

        public int Bauxite { get; set; }
        public int BauxiteDifference => Previous != null ? Bauxite - Previous.Bauxite : 0;

        public int InstantConstruction { get; set; }
        public int InstantConstructionDifference => Previous != null ? InstantConstruction - Previous.InstantConstruction : 0;

        public int Bucket { get; set; }
        public int BucketDifference => Previous != null ? Bucket - Previous.Bucket : 0;

        public int DevelopmentMaterial { get; set; }
        public int DevelopmentMaterialDifference => Previous != null ? DevelopmentMaterial - Previous.DevelopmentMaterial : 0;

        public int ImprovementMaterial { get; set; }
        public int ImprovementMaterialDifference => Previous != null ? ImprovementMaterial - Previous.ImprovementMaterial : 0;

        public ResourceRecord(SQLiteDataReader rpReader)
        {
            ID = rpReader.GetInt64("time");

            Fuel = rpReader.GetInt32("fuel");
            Bullet = rpReader.GetInt32("bullet");
            Steel = rpReader.GetInt32("steel");
            Bauxite = rpReader.GetInt32("bauxite");
            InstantConstruction = rpReader.GetInt32("instant_construction");
            Bucket = rpReader.GetInt32("bucket");
            DevelopmentMaterial = rpReader.GetInt32("development_material");
            ImprovementMaterial = rpReader.GetInt32("improvement_material");
        }

        public void Update() => OnPropertyChanged(string.Empty);
    }
}
