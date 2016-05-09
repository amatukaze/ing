using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AirForceGroup : RawDataWrapper<RawAirForceGroup>, IID
    {
        public int ID => RawData.ID;

        string r_Name;
        public string Name
        {
            get { return r_Name; }
            internal set
            {
                if (r_Name != value)
                {
                    r_Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        int r_CombatRadius;
        public int CombatRadius
        {
            get { return r_CombatRadius; }
            set
            {
                if (r_CombatRadius != value)
                {
                    r_CombatRadius = value;
                    OnPropertyChanged(nameof(CombatRadius));
                }
            }
        }

        AirForceGroupOption r_Option;
        public AirForceGroupOption Option
        {
            get { return r_Option; }
            internal set
            {
                if (r_Option != value)
                {
                    r_Option = value;
                    OnPropertyChanged(nameof(Option));
                }
            }
        }

        public IDTable<AirForceSquadron> Squadrons { get; } = new IDTable<AirForceSquadron>();

        internal protected AirForceGroup(RawAirForceGroup rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            Name = RawData.Name;
            CombatRadius = RawData.CombatRadius;
            Option = RawData.Option;

            Squadrons.UpdateRawData(RawData.Squadrons, r => new AirForceSquadron(r), (rpData, rpRawData) => rpData.Update(rpRawData));
        }
    }
}
