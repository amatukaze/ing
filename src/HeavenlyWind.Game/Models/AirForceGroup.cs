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

        public int AreaId => RawData.AreaID;

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

        int _combatRadiusBase;
        public int CombatRadiusBase
        {
            get => _combatRadiusBase;
            internal set
            {
                if (_combatRadiusBase != value)
                {
                    _combatRadiusBase = value;
                    OnPropertyChanged();
                }
            }
        }
        int _combatRadiusBonus;
        public int CombatRadiusBonus
        {
            get => _combatRadiusBonus;
            internal set
            {
                if (_combatRadiusBonus != value)
                {
                    _combatRadiusBonus = value;
                    OnPropertyChanged();
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

        int r_HighDefenseFighterPower;
        public int HighDefenseFighterPower
        {
            get { return r_HighDefenseFighterPower; }
            internal set
            {
                if (r_HighDefenseFighterPower != value)
                {
                    r_HighDefenseFighterPower = value;
                    OnPropertyChanged();
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

                    UpdateFighterPower();
                    UpdateLBASConsumption();
                }
            }
        }

        public int LBASFuelConsumption { get; private set; }
        public int LBASBulletConsumption { get; private set; }

        public IDTable<AirForceSquadron> Squadrons { get; } = new IDTable<AirForceSquadron>();

        public AirForceSquadronRelocationCountdown Relocation { get; private set; }

        internal protected AirForceGroup(RawAirForceGroup rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            Name = RawData.Name;
            Option = RawData.Option;

            Squadrons.UpdateRawData(RawData.Squadrons, r => new AirForceSquadron(this, r), (rpData, rpRawData) => rpData.Update(rpRawData));

            CombatRadiusBase = RawData.CombatRadius.Base;
            CombatRadiusBonus = RawData.CombatRadius.Bonus;

            UpdateFighterPower();
            UpdateLBASConsumption();
        }

        internal void UpdateFighterPower()
        {
            var rFighterPower = .0;

            EquipmentInfo rReconnaissancePlane = null;

            foreach (var rSquadron in Squadrons.Values)
            {
                if (rSquadron.State != AirForceSquadronState.Idle)
                    continue;

                var rPlane = rSquadron.Plane;
                var rInfo = rPlane.Info;

                switch (rInfo.Type)
                {
                    case EquipmentType.CarrierBasedRecon:
                    case EquipmentType.ReconSeaplane:
                    case EquipmentType.LargeFlyingBoat:
                    case EquipmentType.LandBasedRecon:
                        if (rReconnaissancePlane == null || rReconnaissancePlane.LoS < rInfo.LoS || rReconnaissancePlane.Type > rInfo.Type)
                            rReconnaissancePlane = rInfo;
                        break;
                }

                if (!rInfo.CanParticipateInFighterCombat)
                    continue;

                double rResult;

                if (r_Option == AirForceGroupOption.AirDefense)
                    rResult = rInfo.AA + rInfo.Interception + rInfo.AntiBomber * 2.0;
                else
                    rResult = rInfo.AA + rInfo.Interception * 1.5;

                var rSquareRootOfPlaneCount = Math.Sqrt(rSquadron.Count);

                rResult *= rSquareRootOfPlaneCount;

                switch (rInfo.Type)
                {
                    case EquipmentType.CarrierBasedFighter:
                    case EquipmentType.SeaplaneFighter:
                    case EquipmentType.InterceptorFighter:
                        rResult += rPlane.Level * .2 * rSquareRootOfPlaneCount;
                        break;

                    case EquipmentType.CarrierBasedDiveBomber:
                        rResult += rPlane.Level * .25 * rSquareRootOfPlaneCount;
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
                        case EquipmentType.JetPoweredFighter:
                        case EquipmentType.ASAircraft when rInfo.DiveBomberAttack > 0: // 一式戦 隼II型改(20戦隊)/一式戦 隼III型改(熟練/20戦隊)
                            rResult += r_FighterFPBouns[rProficiency];
                            break;

                        case EquipmentType.SeaplaneBomber:
                            rResult += r_SeaplaneBomberFPBouns[rProficiency];
                            break;
                    }

                    rResult += Math.Sqrt(r_InternalFPBonus[rProficiency] * .1);
                }

                rFighterPower += rResult;
            }

            var rocketCount = 0;

            foreach (var group in KanColleGame.Current.Port.AirBase.Table[AreaId])
            {
                if (group.Option != AirForceGroupOption.AirDefense)
                    continue;

                foreach (var squadron in group.Squadrons)
                    if (squadron.Plane.Info.ID is 350 or 351 or 352)
                        rocketCount++;
            }

            if (rReconnaissancePlane != null)
            {
                if (r_Option is AirForceGroupOption.Sortie && rReconnaissancePlane.Type is EquipmentType.LandBasedRecon)
                    rFighterPower *= rReconnaissancePlane.LoS switch
                    {
                        8 => 1.15,
                        9 => 1.18,
                        _ => 1.0,
                    };
                else if (r_Option is AirForceGroupOption.AirDefense)
                    rFighterPower *= (rReconnaissancePlane.Type, rReconnaissancePlane.LoS) switch
                    {
                        (EquipmentType.ReconSeaplane or EquipmentType.LargeFlyingBoat, < 8) => 1.1,
                        (EquipmentType.ReconSeaplane or EquipmentType.LargeFlyingBoat, 8) => 1.13,
                        (EquipmentType.ReconSeaplane or EquipmentType.LargeFlyingBoat, _) => 1.16,
                        (EquipmentType.CarrierBasedRecon, <= 8) => 1.2,
                        (EquipmentType.CarrierBasedRecon, _) => 1.3,
                        (EquipmentType.LandBasedRecon, 9) => 1.24,
                        (EquipmentType.LandBasedRecon, _) => 1.18,
                        _ => 1.0,
                    };
            }

            FighterPower = (int)rFighterPower;

            if (r_Option is not AirForceGroupOption.AirDefense)
            {
                HighDefenseFighterPower = 0;
                return;
            }

            HighDefenseFighterPower = (int)(rFighterPower * rocketCount switch
            {
                >= 3 => 1.2,
                2 => 1.1,
                1 => 0.8,
                _ => 0.5,
            });
        }

        internal void UpdateRelocationCountdown()
        {
            AirForceSquadronRelocationCountdown rResult = null;

            foreach (var rSquadron in Squadrons.Values)
            {
                if (rSquadron.State != AirForceSquadronState.Relocating || !rSquadron.Relocation.TimeToComplete.HasValue)
                    continue;

                if (rResult == null || rResult.TimeToComplete.Value < rSquadron.Relocation.TimeToComplete.Value)
                    rResult = rSquadron.Relocation;
            }

            Relocation = rResult;
            OnPropertyChanged(nameof(Relocation));
        }

        internal void UpdateLBASConsumption()
        {
            var rFuelConsumption = 0;
            var rBulletConsumption = 0;

            if (r_Option == AirForceGroupOption.Sortie)
                foreach (var rSquadron in Squadrons.Values)
                {
                    if (rSquadron.State != AirForceSquadronState.Idle)
                        continue;

                    var rIcon = rSquadron.Plane.Info.Icon;

                    var rCurrentFuelConsumption = 0;
                    var rCurrentBulletConsumption = 0;

                    switch (rIcon)
                    {
                        case EquipmentIconType.LandBasedAttackAircraft:
                            rCurrentFuelConsumption = 27;
                            rCurrentBulletConsumption = 12;
                            break;

                        case EquipmentIconType.CarrierBasedRecon:
                            rCurrentFuelConsumption = 4;
                            rCurrentBulletConsumption = 3;
                            break;

                        case EquipmentIconType.JetFighterBomberKeiunKai:
                        case EquipmentIconType.JetFighterBomberKikkaKai:
                            rCurrentFuelConsumption += 15;
                            rCurrentBulletConsumption = 8;
                            break;

                        default:
                            rCurrentFuelConsumption = 18;
                            rCurrentBulletConsumption = 11;
                            break;
                    }

                    var rRate = rSquadron.Count / (double)rSquadron.MaxCount;

                    rFuelConsumption += (int)Math.Round(rCurrentFuelConsumption * rRate);

                    if (rIcon == EquipmentIconType.LandBasedAttackAircraft)
                        rBulletConsumption += (int)Math.Round(rCurrentBulletConsumption * rRate);
                    else
                        rBulletConsumption += (int)Math.Ceiling(rCurrentBulletConsumption * rRate);
                }

            LBASFuelConsumption = rFuelConsumption;
            LBASBulletConsumption = rBulletConsumption;

            OnPropertyChanged(nameof(LBASFuelConsumption));
            OnPropertyChanged(nameof(LBASBulletConsumption));
        }
    }
}
