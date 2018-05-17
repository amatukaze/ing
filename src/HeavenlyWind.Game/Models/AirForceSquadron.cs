using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AirForceSquadron : RawDataWrapper<RawAirForceSquadron>, IID
    {
        AirForceGroup r_Group;

        public int ID => RawData.ID;

        public AirForceSquadronState State => RawData.State;

        public Equipment Plane => KanColleGame.Current.Port.Equipment.GetValueOrDefault(RawData.EquipmentID) ?? Equipment.Dummy;

        public int Count => RawData.Count;
        public int MaxCount => RawData.MaxCount;
        public bool NeedResupply => Count != MaxCount;

        public AirForceSquadronCondition Condition => RawData.Condition;

        public AirForceSquadronRelocationCountdown Relocation { get; }

        internal protected AirForceSquadron(AirForceGroup rpGroup, RawAirForceSquadron rpRawData) : base(rpRawData)
        {
            r_Group = rpGroup;

            Relocation = new AirForceSquadronRelocationCountdown(rpGroup, this);
        }

        protected override void OnRawDataUpdated()
        {
            if (State == AirForceSquadronState.Relocating && !Relocation.TimeToComplete.HasValue)
                Relocation.Start();
            else if (State != AirForceSquadronState.Relocating && Relocation.TimeToComplete.HasValue)
                Relocation.Reset();

            OnPropertyChanged(string.Empty);
        }

        internal void RelocationComplete()
        {
            RawData.State = AirForceSquadronState.Empty;
            RawData.EquipmentID = 0;

            OnPropertyChanged(string.Empty);

            r_Group.UpdateRelocationCountdown();
        }
    }
}
