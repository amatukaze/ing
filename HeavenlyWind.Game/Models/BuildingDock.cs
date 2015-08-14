using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public enum BuildingDockState { Locked = -1, Idle, Building = 2, Completed }

    public class BuildingDock : CountdownModelBase, IID
    {
        public int ID { get; }

        BuildingDockState r_State;
        public BuildingDockState State
        {
            get { return r_State; }
            private set
            {
                if (r_State != value)
                {
                    r_State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        ShipInfo r_Ship;
        public ShipInfo Ship
        {
            get { return r_Ship; }
            private set
            {
                if (r_Ship != value)
                {
                    r_Ship = value;
                    OnPropertyChanged(nameof(Ship));
                }
            }
        }

        int r_FuelConsumption;
        public int FuelConsumption
        {
            get { return r_FuelConsumption; }
            private set
            {
                if (r_FuelConsumption != value)
                {
                    r_FuelConsumption = value;
                    OnPropertyChanged(nameof(FuelConsumption));
                }
            }
        }
        int r_BulletConsumption;
        public int BulletConsumption
        {
            get { return r_BulletConsumption; }
            private set
            {
                if (r_BulletConsumption != value)
                {
                    r_BulletConsumption = value;
                    OnPropertyChanged(nameof(BulletConsumption));
                }
            }
        }
        int r_SteelConsumption;
        public int SteelConsumption
        {
            get { return r_SteelConsumption; }
            private set
            {
                if (r_SteelConsumption != value)
                {
                    r_SteelConsumption = value;
                    OnPropertyChanged(nameof(SteelConsumption));
                }
            }
        }
        int r_BauxiteConsumption;
        public int BauxiteConsumption
        {
            get { return r_BauxiteConsumption; }
            private set
            {
                if (r_BauxiteConsumption != value)
                {
                    r_BauxiteConsumption = value;
                    OnPropertyChanged(nameof(BauxiteConsumption));
                }
            }
        }
        int r_DevelopmentMaterialConsumption;
        public int DevelopmentMaterialConsumption
        {
            get { return r_DevelopmentMaterialConsumption; }
            private set
            {
                if (r_DevelopmentMaterialConsumption != value)
                {
                    r_DevelopmentMaterialConsumption = value;
                    OnPropertyChanged(nameof(DevelopmentMaterialConsumption));
                }
            }
        }

        public bool? IsLargeShipConstruction
        {
            get
            {
                if (State == BuildingDockState.Idle || State == BuildingDockState.Locked)
                    return null;

                return FuelConsumption >= 1000 && BulletConsumption >= 1000 && SteelConsumption >= 1000 & BauxiteConsumption >= 1000;
            }
        }

        public event Action<string> BuildingCompleted = delegate { };

        internal BuildingDock(RawBuildingDock rpRawData)
        {
            ID = rpRawData.ID;

            Update(rpRawData);
        }

        public void Update(RawBuildingDock rpRawData)
        {
            State = rpRawData.State;

            FuelConsumption = rpRawData.FuelConsumption;
            BulletConsumption = rpRawData.BulletConsumption;
            SteelConsumption = rpRawData.SteelConsumption;
            BauxiteConsumption = rpRawData.BauxiteConsumption;
            DevelopmentMaterialConsumption = rpRawData.DevelopmentMaterialConsumption;

            if (State == BuildingDockState.Building || State == BuildingDockState.Completed)
            {
                Ship = KanColleGame.Current.MasterInfo.Ships[rpRawData.ShipID];
                CompleteTime = DateTimeUtil.UnixEpoch.AddMilliseconds(rpRawData.CompleteTime);
            }
            else
            {
                Ship = null;
                CompleteTime = null;
            }
        }

        internal void CompleteConstruction()
        {
            IsNotificated = true;
            State = BuildingDockState.Completed;
            CompleteTime = null;
        }

        protected override void TimeOut()
        {
            State = BuildingDockState.Completed;
            BuildingCompleted(Ship.Name);
        }

        public override string ToString()
        {
            var rBuilder = new StringBuilder(64);
            rBuilder.Append($"ID = {ID}, State = {State}");
            if (State == BuildingDockState.Building || State == BuildingDockState.Completed)
                rBuilder.Append($", Ship = \"{Ship.Name}\"");
            if (State == BuildingDockState.Building)
                rBuilder.Append($", CompleteTime = \"{CompleteTime.Value}\"");

            return rBuilder.ToString();
        }
    }
}
