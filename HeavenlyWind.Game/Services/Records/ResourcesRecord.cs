using System;
using System.Data.SQLite;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ResourcesRecord : RecordBase
    {
        public override string TableName => "resources";
        public override int Version => 1;

        int r_Fuel;
        int r_Bullet;
        int r_Steel;
        int r_Bauxite;
        int r_DevelopmentMaterial;
        int r_Bucket;
        int r_InstantConstruction;
        int r_ImprovementMaterial;

        Subject<Unit> r_UpdateSource = new Subject<Unit>();
        DateTime r_LastUpdateTime;

        internal ResourcesRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_port/port", _ =>
            {
                var rMaterials = KanColleGame.Current.Port.Materials;

                var rShouldInsertRecord =
                    r_Fuel != rMaterials.Fuel ||
                    r_Bullet != rMaterials.Bullet ||
                    r_Steel != rMaterials.Steel ||
                    r_Bauxite != rMaterials.Bauxite ||
                    r_DevelopmentMaterial != rMaterials.DevelopmentMaterial ||
                    r_Bucket != rMaterials.Bucket ||
                    r_InstantConstruction != rMaterials.InstantConstruction ||
                    r_ImprovementMaterial != rMaterials.ImprovementMaterial;

                r_Fuel = rMaterials.Fuel;
                r_Bullet = rMaterials.Bullet;
                r_Steel = rMaterials.Steel;
                r_Bauxite = rMaterials.Bauxite;
                r_DevelopmentMaterial = rMaterials.DevelopmentMaterial;
                r_Bucket = rMaterials.Bucket;
                r_InstantConstruction = rMaterials.InstantConstruction;
                r_ImprovementMaterial = rMaterials.ImprovementMaterial;

                if (rShouldInsertRecord)
                    r_UpdateSource.OnNext(Unit.Default);
            }));

            DisposableObjects.Add(r_UpdateSource.Subscribe(_ => InsertRecord()));
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
                    "development_material INTEGER NOT NULL, " +
                    "bucket INTEGER NOT NULL, " +
                    "instant_construction INTEGER NOT NULL, " +
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
                        r_DevelopmentMaterial = Convert.ToInt32(rReader["development_material"]);
                        r_Bucket = Convert.ToInt32(rReader["bucket"]);
                        r_InstantConstruction = Convert.ToInt32(rReader["instant_construction"]);
                        r_ImprovementMaterial = Convert.ToInt32(rReader["improvement_material"]);
                    }
            }
        }

        void InsertRecord()
        {
            if ((DateTime.Now - r_LastUpdateTime).TotalMinutes < 2.0)
                return;

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO resources(time, fuel, bullet, steel, bauxite, development_material, bucket, instant_construction, improvement_material) " +
                    "VALUES(strftime('%s', 'now'), @fuel, @bullet, @steel, @bauxite, @development_material, @bucket, @instant_construction, @improvement_material);";
                rCommand.Parameters.AddWithValue("@fuel", r_Fuel);
                rCommand.Parameters.AddWithValue("@bullet", r_Bullet);
                rCommand.Parameters.AddWithValue("@steel", r_Steel);
                rCommand.Parameters.AddWithValue("@bauxite", r_Bauxite);
                rCommand.Parameters.AddWithValue("@development_material", r_DevelopmentMaterial);
                rCommand.Parameters.AddWithValue("@bucket", r_Bucket);
                rCommand.Parameters.AddWithValue("@instant_construction", r_InstantConstruction);
                rCommand.Parameters.AddWithValue("@improvement_material", r_ImprovementMaterial);

                rCommand.ExecuteNonQuery();
            }

            r_LastUpdateTime = DateTime.Now;
        }
    }
}
