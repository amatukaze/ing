using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Materials : ModelBase
    {
        int r_Fuel;
        public int Fuel
        {
            get { return r_Fuel; }
            internal set
            {
                if (r_Fuel != value)
                {
                    r_Fuel = value;
                    OnPropertyChanged(nameof(Fuel));
                }
            }
        }
        int r_Bullet;
        public int Bullet
        {
            get { return r_Bullet; }
            internal set
            {
                if (r_Bullet != value)
                {
                    r_Bullet = value;
                    OnPropertyChanged(nameof(Bullet));
                }
            }
        }
        int r_Steel;
        public int Steel
        {
            get { return r_Steel; }
            internal set
            {
                if (r_Steel != value)
                {
                    r_Steel = value;
                    OnPropertyChanged(nameof(Steel));
                }
            }
        }
        int r_Bauxite;
        public int Bauxite
        {
            get { return r_Bauxite; }
            internal set
            {
                if (r_Bauxite != value)
                {
                    r_Bauxite = value;
                    OnPropertyChanged(nameof(Bauxite));
                }
            }
        }
        int r_DevelopmentMaterial;
        public int DevelopmentMaterial
        {
            get { return r_DevelopmentMaterial; }
            internal set
            {
                if (r_DevelopmentMaterial != value)
                {
                    r_DevelopmentMaterial = value;
                    OnPropertyChanged(nameof(DevelopmentMaterial));
                }
            }
        }
        int r_Bucket;
        public int Bucket
        {
            get { return r_Bucket; }
            internal set
            {
                if (r_Bucket != value)
                {
                    r_Bucket = value;
                    OnPropertyChanged(nameof(Bucket));
                }
            }
        }
        int r_InstantConstruction;
        public int InstantConstruction
        {
            get { return r_InstantConstruction; }
            internal set
            {
                if (r_InstantConstruction != value)
                {
                    r_InstantConstruction = value;
                    OnPropertyChanged(nameof(InstantConstruction));
                }
            }
        }
        int r_ImprovementMaterial;
        public int ImprovementMaterial
        {
            get { return r_ImprovementMaterial; }
            internal set
            {
                if (r_ImprovementMaterial != value)
                {
                    r_ImprovementMaterial = value;
                    OnPropertyChanged(nameof(ImprovementMaterial));
                }
            }
        }

        internal void Update(RawMaterial[] rpData)
        {
            if (rpData?.Length >= 8)
            {
                Fuel = rpData[0].Amount;
                Bullet = rpData[1].Amount;
                Steel = rpData[2].Amount;
                Bauxite = rpData[3].Amount;
                InstantConstruction = rpData[4].Amount;
                Bucket = rpData[5].Amount;
                DevelopmentMaterial = rpData[6].Amount;
                ImprovementMaterial = rpData[7].Amount;
            }
        }
        internal void Update(int[] rpData)
        {
            if (rpData?.Length >= 4)
            {
                Fuel = rpData[0];
                Bullet = rpData[1];
                Steel = rpData[2];
                Bauxite = rpData[3];

                if (rpData.Length == 8)
                {
                    InstantConstruction = rpData[4];
                    Bucket = rpData[5];
                    DevelopmentMaterial = rpData[6];
                    ImprovementMaterial = rpData[7];
                }
            }
        }

        public override string ToString() => $"{Fuel}, {Bullet}, {Steel}, {Bauxite}";
    }
}
