using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Reactive.Linq;

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
                    UpdateDifference(MaterialType.Fuel);
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
                    UpdateDifference(MaterialType.Bullet);
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
                    UpdateDifference(MaterialType.Steel);
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
                    UpdateDifference(MaterialType.Bauxite);
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
                    UpdateDifference(MaterialType.InstantConstruction);
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
                    UpdateDifference(MaterialType.Bucket);
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
                    UpdateDifference(MaterialType.DevelopmentMaterial);
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
                    UpdateDifference(MaterialType.ImprovementMaterial);
                }
            }
        }

        public MaterialsDifference DayDifference { get; private set; }
        public MaterialsDifference WeekDifference { get; private set; }
        public MaterialsDifference MonthDifference { get; private set; }

        internal Materials()
        {
            SessionService.Instance.SubscribeOnce("api_port/port", delegate
            {
                DayDifference = new MaterialsDifference(this, MaterialsDifferenceType.Day);
                WeekDifference = new MaterialsDifference(this, MaterialsDifferenceType.Week);
                MonthDifference = new MaterialsDifference(this, MaterialsDifferenceType.Month);

                UpdateCore();

                Observable.Timer(new DateTimeOffset(DateTime.Now.Date.AddDays(1.0)), TimeSpan.FromDays(1.0)).Subscribe(delegate
                {
                    DayDifference.Reload();
                    WeekDifference.Reload();
                    MonthDifference.Reload();

                    OnPropertyChanged(nameof(DayDifference));
                    OnPropertyChanged(nameof(WeekDifference));
                    OnPropertyChanged(nameof(MonthDifference));
                });
            });
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

                UpdateCore();
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

                UpdateCore();
            }
        }
        void UpdateCore()
        {
            OnPropertyChanged(nameof(DayDifference));
            OnPropertyChanged(nameof(WeekDifference));
            OnPropertyChanged(nameof(MonthDifference));
        }

        public void UpdateDifference(MaterialType rpType)
        {
            DayDifference?.Update(rpType);
            WeekDifference?.Update(rpType);
            MonthDifference?.Update(rpType);
        }

        public override string ToString() => $"{Fuel}, {Bullet}, {Steel}, {Bauxite}";
    }
}
