using System.Linq;
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

        int r_MinCombatRadius;
        public int MinCombatRadius
        {
            get { return r_MinCombatRadius; }
            set
            {
                if (r_MinCombatRadius != value)
                {
                    r_MinCombatRadius = value;
                    OnPropertyChanged(nameof(MinCombatRadius));
                }
            }
        }
        int r_MaxCombatRadius;
        public int MaxCombatRadius
        {
            get { return r_MaxCombatRadius; }
            set
            {
                if (r_MaxCombatRadius != value)
                {
                    r_MaxCombatRadius = value;
                    OnPropertyChanged(nameof(MaxCombatRadius));
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
            Option = RawData.Option;

            Squadrons.UpdateRawData(RawData.Squadrons, r => new AirForceSquadron(r), (rpData, rpRawData) => rpData.Update(rpRawData));

            UpdateCombatRadius();
        }

        internal void UpdateCombatRadius()
        {
            var rSquadrons = Squadrons.Values.Where(r => r.State == AirForceSquadronState.Idle);
            if (!rSquadrons.Any())
                return;

            MinCombatRadius = rSquadrons.Min(r => r.Plane.Info.CombatRadius);
            MaxCombatRadius = rSquadrons.Max(r => r.Plane.Info.CombatRadius);
        }
    }
}
