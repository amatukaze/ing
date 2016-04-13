using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class MaterialsDifference : ModelBase
    {
        Materials r_Owner;

        public MaterialsDifferenceType Type { get; }

        int r_Fuel;
        public int Fuel => r_Owner.Fuel - r_Fuel;

        int r_Bullet;
        public int Bullet => r_Owner.Bullet - r_Bullet;

        int r_Steel;
        public int Steel => r_Owner.Steel - r_Steel;

        int r_Bauxite;
        public int Bauxite => r_Owner.Bauxite - r_Bauxite;

        int r_DevelopmentMaterial;
        public int DevelopmentMaterial => r_Owner.DevelopmentMaterial - r_DevelopmentMaterial;

        int r_Bucket;
        public int Bucket => r_Owner.Bucket - r_Bucket;

        int r_InstantConstruction;
        public int InstantConstruction => r_Owner.InstantConstruction - r_InstantConstruction;

        int r_ImprovementMaterial;
        public int ImprovementMaterial => r_Owner.ImprovementMaterial - r_ImprovementMaterial;

        internal MaterialsDifference(Materials rpOwner, MaterialsDifferenceType rpType)
        {
            r_Owner = rpOwner;

            Type = rpType;

            Initialize();
        }
        void Initialize()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                switch (Type)
                {
                    case MaterialsDifferenceType.Day:
                        rCommand.CommandText = "SELECT * FROM resources WHERE time >= strftime('%s', 'now', 'start of day') LIMIT 1";
                        break;

                    case MaterialsDifferenceType.Week:
                        rCommand.CommandText = "SELECT * FROM resources WHERE time >= strftime('%s', 'now', 'weekday 1', '-7 days', 'start of day') LIMIT 1";
                        break;

                    case MaterialsDifferenceType.Month:
                        rCommand.CommandText = "SELECT * FROM resources WHERE time >= strftime('%s', 'now', 'start of month') LIMIT 1";
                        break;
                }

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

        internal void Update(MaterialType rpType)
        {
            switch (rpType)
            {
                case MaterialType.Fuel:
                    OnPropertyChanged(nameof(Fuel));
                    break;

                case MaterialType.Bullet:
                    OnPropertyChanged(nameof(Bullet));
                    break;

                case MaterialType.Steel:
                    OnPropertyChanged(nameof(Steel));
                    break;

                case MaterialType.Bauxite:
                    OnPropertyChanged(nameof(Bauxite));
                    break;

                case MaterialType.DevelopmentMaterial:
                    OnPropertyChanged(nameof(DevelopmentMaterial));
                    break;

                case MaterialType.Bucket:
                    OnPropertyChanged(nameof(Bucket));
                    break;

                case MaterialType.InstantConstruction:
                    OnPropertyChanged(nameof(InstantConstruction));
                    break;

                case MaterialType.ImprovementMaterial:
                    OnPropertyChanged(nameof(ImprovementMaterial));
                    break;
            }
        }
    }
}
