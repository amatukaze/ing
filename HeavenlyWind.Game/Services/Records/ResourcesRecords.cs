using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    class ResourcesRecords : RecordsGroup
    {
        public override string GroupName => "resources";

        int r_Fuel;
        int r_Bullet;
        int r_Steel;
        int r_Bauxite;
        int r_InstantConstruction;
        int r_Bucket;
        int r_DevelopmentMaterial;
        int r_ImprovementMaterial;

        internal ResourcesRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            var rApis = new[]
            {
                "api_port/port",
                "api_req_map/start",
            };
            DisposableObjects.Add(ApiService.Subscribe(rApis, r =>
            {
                var rMaterials = KanColleGame.Current.Port.Materials;

                var rShouldInsertRecord =
                    r_Fuel != rMaterials.Fuel ||
                    r_Bullet != rMaterials.Bullet ||
                    r_Steel != rMaterials.Steel ||
                    r_Bauxite != rMaterials.Bauxite ||
                    r_InstantConstruction != rMaterials.InstantConstruction ||
                    r_Bucket != rMaterials.Bucket ||
                    r_DevelopmentMaterial != rMaterials.DevelopmentMaterial ||
                    r_ImprovementMaterial != rMaterials.ImprovementMaterial;

                if (rShouldInsertRecord)
                {
                    r_Fuel = rMaterials.Fuel;
                    r_Bullet = rMaterials.Bullet;
                    r_Steel = rMaterials.Steel;
                    r_Bauxite = rMaterials.Bauxite;
                    r_InstantConstruction = rMaterials.InstantConstruction;
                    r_Bucket = rMaterials.Bucket;
                    r_DevelopmentMaterial = rMaterials.DevelopmentMaterial;
                    r_ImprovementMaterial = rMaterials.ImprovementMaterial;

                    InsertRecord(r.Timestamp);
                }
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS resources(" +
                    "time INTEGER PRIMARY KEY NOT NULL, " +
                    "fuel INTEGER NOT NULL, " +
                    "bullet INTEGER NOT NULL, " +
                    "steel INTEGER NOT NULL, " +
                    "bauxite INTEGER NOT NULL, " +
                    "instant_construction INTEGER NOT NULL, " +
                    "bucket INTEGER NOT NULL, " +
                    "development_material INTEGER NOT NULL, " +
                    "improvement_material INTEGER NOT NULL);";

                rCommand.ExecuteNonQuery();
            }
        }
        protected override void Load()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM resources ORDER BY time DESC LIMIT 1;";

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                    {
                        r_Fuel = Convert.ToInt32(rReader["fuel"]);
                        r_Bullet = Convert.ToInt32(rReader["bullet"]);
                        r_Steel = Convert.ToInt32(rReader["steel"]);
                        r_Bauxite = Convert.ToInt32(rReader["bauxite"]);
                        r_InstantConstruction = Convert.ToInt32(rReader["instant_construction"]);
                        r_Bucket = Convert.ToInt32(rReader["bucket"]);
                        r_DevelopmentMaterial = Convert.ToInt32(rReader["development_material"]);
                        r_ImprovementMaterial = Convert.ToInt32(rReader["improvement_material"]);
                    }
            }
        }

        void InsertRecord(long rpTimestamp)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO resources(time, fuel, bullet, steel, bauxite, instant_construction, bucket, development_material, improvement_material) " +
                    "VALUES(@time, @fuel, @bullet, @steel, @bauxite, @instant_construction, @bucket, @development_material, @improvement_material);";
                rCommand.Parameters.AddWithValue("@time", rpTimestamp);
                rCommand.Parameters.AddWithValue("@fuel", r_Fuel);
                rCommand.Parameters.AddWithValue("@bullet", r_Bullet);
                rCommand.Parameters.AddWithValue("@steel", r_Steel);
                rCommand.Parameters.AddWithValue("@bauxite", r_Bauxite);
                rCommand.Parameters.AddWithValue("@instant_construction", r_InstantConstruction);
                rCommand.Parameters.AddWithValue("@bucket", r_Bucket);
                rCommand.Parameters.AddWithValue("@development_material", r_DevelopmentMaterial);
                rCommand.Parameters.AddWithValue("@improvement_material", r_ImprovementMaterial);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
