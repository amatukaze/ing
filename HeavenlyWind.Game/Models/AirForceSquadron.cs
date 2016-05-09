using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

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

        internal protected AirForceSquadron(RawAirForceSquadron rpRawData) : base(rpRawData)
        {
        }

        protected override void OnRawDataUpdated()
        {
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Plane));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(MaxCount));
            OnPropertyChanged(nameof(NeedResupply));
            OnPropertyChanged(nameof(Condition));
        }
    }
}
