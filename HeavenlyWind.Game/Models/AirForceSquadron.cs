using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AirForceSquadron : RawDataWrapper<RawAirForceSquadron>, IID
    {
        public int ID => RawData.ID;

        public AirForceSquadronState State => RawData.State;

        public Equipment Plane => RawData.EquipmentID != 0 ? KanColleGame.Current.Port.Equipment[RawData.EquipmentID] : null;

        public int Count => RawData.Count;
        public int MaxCount => RawData.MaxCount;
        public bool NeedResupply => Count != MaxCount;

        public AirForceSquadronCondition Condition => RawData.Condition;

        IDisposable r_Relocating;

        internal protected AirForceSquadron(RawAirForceSquadron rpRawData) : base(rpRawData)
        {
        }

        protected override void OnRawDataUpdated()
        {
            if (State == AirForceSquadronState.Relocating)
                r_Relocating = Observable.Timer(TimeSpan.FromMinutes(20.0)).Subscribe(delegate
                {
                    RawData.State = AirForceSquadronState.Empty;
                    RawData.EquipmentID = 0;

                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(Plane));

                    r_Relocating.Dispose();
                    r_Relocating = null;
                });
            else if (r_Relocating != null)
            {
                r_Relocating.Dispose();
                r_Relocating = null;
            }

            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Plane));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(MaxCount));
            OnPropertyChanged(nameof(NeedResupply));
            OnPropertyChanged(nameof(Condition));
        }
    }
}
