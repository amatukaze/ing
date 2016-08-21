using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AirForceGroup : RawDataWrapper<RawAirForceGroup>, IID
    {
        static int[] r_FighterFPBouns = { 0, 0, 2, 5, 9, 14, 14, 22 };
        static int[] r_SeaplaneBomberFPBouns = { 0, 0, 1, 1, 1, 3, 3, 6 };
        static int[] r_InternalFPBonus = { 10, 25, 40, 55, 70, 85, 100, 120 };

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
            internal set
            {
                if (r_CombatRadius != value)
                {
                    r_CombatRadius = value;
                    OnPropertyChanged(nameof(CombatRadius));
                }
            }
        }

        int r_FighterPower;
        public int FighterPower
        {
            get { return r_FighterPower; }
            internal set
            {
                if (r_FighterPower != value)
                {
                    r_FighterPower = value;
                    OnPropertyChanged(nameof(FighterPower));
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

            CombatRadius = RawData.CombatRadius;

            UpdateFighterPower();
        }

        internal void UpdateFighterPower()
        {
            var rFighterPower = 0;

            foreach (var rSquadron in Squadrons.Values)
            {
                if (rSquadron.State != AirForceSquadronState.Idle)
                    continue;

                var rPlane = rSquadron.Plane;
                var rInfo = rPlane.Info;

                if (!rInfo.CanParticipateInFighterCombat)
                    continue;

                var rResult = (rInfo.AA + rInfo.Interception * 1.5) * Math.Sqrt(rSquadron.Count);
                rFighterPower += (int)rResult;

                switch (rInfo.Type)
                {
                    case EquipmentType.CarrierBasedFighter:
                        rResult += rPlane.Level * .2 * Math.Sqrt(rSquadron.Count);
                        break;
                    case EquipmentType.CarrierBasedDiveBomber:
                        rResult += rPlane.Level * .25 * Math.Sqrt(rSquadron.Count);
                        break;
                }

                if (rSquadron.Count > 0)
                {
                    var rProficiency = rPlane.Proficiency;

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedFighter:
                        case EquipmentType.SeaplaneFighter:
                        case EquipmentType.InterceptorFighter:
                            rResult += r_FighterFPBouns[rProficiency];
                            break;

                        case EquipmentType.SeaplaneBomber:
                            rResult += r_SeaplaneBomberFPBouns[rProficiency];
                            break;
                    }

                    rResult += Math.Sqrt(r_InternalFPBonus[rProficiency] / 10.0);
                }
                rFighterPower += (int)rResult;
            }

            FighterPower = rFighterPower;
        }
    }
}
