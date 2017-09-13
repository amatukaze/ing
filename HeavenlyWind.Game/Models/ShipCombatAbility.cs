using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipCombatAbility : ModelBase
    {
        Ship r_Ship;

        public AttackMode DayBattleAttackMode { get; private set; }
        public CutInType DayBattleCutInType { get; private set; }

        public int DayBattleOpeningAerialStrikePower { get; private set; }
        public int DayBattleShellingPower { get; private set; }
        public int DayBattleCarrierShellingPower { get; private set; }

        public AttackMode NightBattleAttackMode { get; private set; }
        public CutInType NightBattleCutInType { get; private set; }

        public int NightBattleAttackPower { get; private set; }

        public int TorpedoAttackPower { get; private set; }

        public bool CanParticipateInASW { get; private set; }
        public int ASWAttackPower { get; private set; }

        internal ShipCombatAbility(Ship rpOwner)
        {
            r_Ship = rpOwner;
        }

        internal void Update()
        {
            GetAttackMode();

            CalculateDayBattleOpeningAerialStrikeAttackPower();
            CalculateDayBattleAttackPower();
            CalculateDayBattleCarrierAttackPower();

            CalculateNightBattleAttackPower();

            CalculateTorpedoAttackPower();

            CheckCanParticipantInASW();
            CalculateASWAttackPower();

            OnPropertyChanged(nameof(DayBattleAttackMode));
            OnPropertyChanged(nameof(DayBattleCutInType));
            OnPropertyChanged(nameof(DayBattleOpeningAerialStrikePower));
            OnPropertyChanged(nameof(DayBattleShellingPower));
            OnPropertyChanged(nameof(DayBattleCarrierShellingPower));
            OnPropertyChanged(nameof(NightBattleAttackMode));
            OnPropertyChanged(nameof(NightBattleCutInType));
            OnPropertyChanged(nameof(NightBattleAttackPower));
            OnPropertyChanged(nameof(TorpedoAttackPower));
            OnPropertyChanged(nameof(CanParticipateInASW));
            OnPropertyChanged(nameof(ASWAttackPower));
        }

        void GetAttackMode()
        {
            var rMainGunCount = 0;
            var rSecondaryGunCount = 0;
            var rTorpedoCount = 0;
            var rSeaplaneCount = 0;
            var rRadarCount = 0;
            var rAPShellCount = 0;

            foreach (var rEquipment in r_Ship.EquipedEquipment)
            {
                switch (rEquipment.Info.Type)
                {
                    case EquipmentType.SmallCaliberGun:
                    case EquipmentType.MediumCaliberGun:
                    case EquipmentType.LargeCaliberGun:
                        rMainGunCount++;
                        break;

                    case EquipmentType.SecondaryGun:
                        rSecondaryGunCount++;
                        break;

                    case EquipmentType.Torpedo:
                    case EquipmentType.SubmarineTorpedo:
                        rTorpedoCount++;
                        break;

                    case EquipmentType.ReconSeaplane:
                    case EquipmentType.SeaplaneBomber:
                        rSeaplaneCount++;
                        break;

                    case EquipmentType.SmallRadar:
                    case EquipmentType.LargeRadar:
                        rRadarCount++;
                        break;

                    case EquipmentType.APShell:
                        rAPShellCount++;
                        break;
                }
            }

            GetDayBattleAttackMode(rMainGunCount, rSecondaryGunCount, rSeaplaneCount, rRadarCount, rAPShellCount);
            GetNightBattleAttackMode(rMainGunCount, rSecondaryGunCount, rTorpedoCount);
        }
        void GetDayBattleAttackMode(int rpMainGunCount, int rpSecondaryGunCount, int rpSeaplaneCount, int rpRadarCount, int rpAPShellCount)
        {
            DayBattleAttackMode = AttackMode.None;
            DayBattleCutInType = CutInType.None;

            if (rpSeaplaneCount > 0 && rpMainGunCount > 0)
            {
                if (rpMainGunCount == 2 && rpAPShellCount == 1)
                {
                    DayBattleAttackMode = AttackMode.CutIn;
                    DayBattleCutInType = CutInType.DoubleMainGun;
                }
                else if (rpMainGunCount == 1 && rpSecondaryGunCount == 1 && rpAPShellCount == 1)
                {
                    DayBattleAttackMode = AttackMode.CutIn;
                    DayBattleCutInType = CutInType.MainGunAndAPShell;
                }
                else if (rpMainGunCount == 1 && rpSecondaryGunCount == 1 && rpRadarCount == 1)
                {
                    DayBattleAttackMode = AttackMode.CutIn;
                    DayBattleCutInType = CutInType.MainGunAndRadar;
                }
                else if (rpMainGunCount >= 1 && rpSecondaryGunCount >= 1)
                {
                    DayBattleAttackMode = AttackMode.CutIn;
                    DayBattleCutInType = CutInType.MainGunAndSecondaryGun;
                }
                else if (rpMainGunCount >= 2)
                {
                    DayBattleAttackMode = AttackMode.DoubleAttack;
                    DayBattleCutInType = CutInType.None;
                }
            }

            switch ((ShipType)r_Ship.Info.Type.ID)
            {
                case ShipType.LightAircraftCarrier:
                case ShipType.AircraftCarrier:
                case ShipType.ArmoredAircraftCarrier:
                    DayBattleAttackMode = AttackMode.AerialStrike;
                    break;

                case ShipType.Submarine:
                case ShipType.SubmarineAircraftCarrier:
                    DayBattleAttackMode = AttackMode.Torpedo;
                    break;
            }

            // 速吸改 / Hayasui-Kai
            if (r_Ship.Info.ID == 352 && r_Ship.EquipedEquipment.Any(r => r.Info.Type == EquipmentType.CarrierBasedTorpedoBomber))
                DayBattleAttackMode = AttackMode.AerialStrike;

            if (DayBattleAttackMode == AttackMode.None)
                DayBattleAttackMode = AttackMode.SingleAttack;
        }
        void GetNightBattleAttackMode(int rpMainGunCount, int rpSecondaryGunCount, int rpTorpedoCount)
        {
            NightBattleAttackMode = AttackMode.None;
            NightBattleCutInType = CutInType.None;

            if (rpTorpedoCount >= 2)
            {
                NightBattleAttackMode = AttackMode.CutIn;
                NightBattleCutInType = CutInType.DoubleTorpedo;
            }
            else if (rpMainGunCount >= 3)
            {
                NightBattleAttackMode = AttackMode.CutIn;
                NightBattleCutInType = CutInType.TripleMainGun;
            }
            else if (rpMainGunCount == 2 && rpSecondaryGunCount > 0)
            {
                NightBattleAttackMode = AttackMode.CutIn;
                NightBattleCutInType = CutInType.DoubleMainGunAndSecondaryGun;
            }
            else if (rpMainGunCount >= 1 && rpTorpedoCount >= 1)
            {
                NightBattleAttackMode = AttackMode.CutIn;
                NightBattleCutInType = CutInType.Mixed;
            }
            else if ((rpMainGunCount == 2 && rpSecondaryGunCount == 0 && rpTorpedoCount == 0) || (rpMainGunCount == 1 && rpSecondaryGunCount >= 1) || (rpSecondaryGunCount >= 2 && rpTorpedoCount <= 1))
            {
                NightBattleAttackMode = AttackMode.DoubleAttack;
                NightBattleCutInType = CutInType.None;
            }

            switch ((ShipType)r_Ship.Info.Type.ID)
            {
                case ShipType.LightAircraftCarrier:
                case ShipType.AircraftCarrier:
                case ShipType.ArmoredAircraftCarrier:
                    NightBattleAttackMode = AttackMode.None;
                    break;

                case ShipType.Submarine:
                case ShipType.SubmarineAircraftCarrier:
                    NightBattleAttackMode = AttackMode.Torpedo;
                    break;
            }

            // Graf Zeppelin(改) / Graf Zeppelin(-Kai)
            if (r_Ship.Info.ID == 353 || r_Ship.Info.ID == 432)
                NightBattleAttackMode = AttackMode.SingleAttack;

            if (NightBattleAttackMode == AttackMode.None)
                NightBattleAttackMode = AttackMode.SingleAttack;
        }

        void CalculateDayBattleOpeningAerialStrikeAttackPower()
        {
            var rResult = .0;

            foreach (var rSlot in r_Ship.Slots)
            {
                if (!rSlot.HasEquipment)
                    continue;

                var rPlanePower = .0;
                switch (rSlot.Equipment.Info.Type)
                {
                    case EquipmentType.CarrierBasedDiveBomber:
                    case EquipmentType.SeaplaneBomber:
                        rPlanePower = rSlot.Equipment.Info.DiveBomberAttack * Math.Sqrt(rSlot.PlaneCount) + 25;
                        break;

                    case EquipmentType.CarrierBasedTorpedoBomber:
                        rPlanePower = rSlot.Equipment.Info.Torpedo * Math.Sqrt(rSlot.PlaneCount) + 25;
                        rPlanePower *= 1.5;
                        break;

                    default:
                        continue;
                }

                rResult = Math.Max(rPlanePower, rResult);
            }

            rResult = GetAttackPowerAfterCaps(rResult, 150.0);

            DayBattleOpeningAerialStrikePower = (int)rResult;
        }
        void CalculateDayBattleAttackPower()
        {
            DayBattleShellingPower = 0;

            if (DayBattleAttackMode != AttackMode.None && DayBattleAttackMode <= AttackMode.CutIn)
            {
                var rResult = r_Ship.Status.Firepower + GetDayBattleAttackPowerBonusFromImprovedEquipment() + 5.0;
                rResult *= GetHealthModifier();
                rResult += GetAttackPowerBonusFromLightCruiserFitGun();

                rResult = GetAttackPowerAfterCaps(rResult, 180.0);

                rResult *= GetArtillerySpottingModifier();

                DayBattleShellingPower = (int)rResult;
            }
        }
        void CalculateDayBattleCarrierAttackPower()
        {
            DayBattleCarrierShellingPower = 0;

            if (DayBattleAttackMode != AttackMode.AerialStrike)
                return;

            var rStatus = r_Ship.Status;
            var rResult = Math.Floor((rStatus.Firepower + rStatus.Torpedo + Math.Floor(Math.Max(r_Ship.EquipedEquipment.Sum(r => r.Info.DiveBomberAttack), 0) * 1.3) + GetDayBattleAttackPowerBonusFromImprovedEquipment()) * 1.5) + 55;
            rResult *= GetHealthModifier();

            rResult = GetAttackPowerAfterCaps(rResult, 150.0);

            DayBattleCarrierShellingPower = (int)rResult;
        }

        void CalculateNightBattleAttackPower()
        {
            var rStatus = r_Ship.Status;
            var rResult = rStatus.Firepower + rStatus.Torpedo + GetNightBattleAttackPowerBonusFromImprovedEquipment();
            rResult *= GetHealthModifier();
            rResult *= GetNightSpecialAttackModifier();
            rResult += GetAttackPowerBonusFromLightCruiserFitGun();

            rResult = GetAttackPowerAfterCaps(rResult, 300.0);

            NightBattleAttackPower = (int)rResult;
        }

        void CalculateTorpedoAttackPower()
        {
            TorpedoAttackPower = 0;

            var rStatus = r_Ship.Status;
            if (rStatus.TorpedoBase.Current == 0)
                return;

            var rResult = rStatus.Torpedo + GetTorpedoAttackPowerBonusFromImprovedEquipment() + 5.0;
            rResult *= GetHealthModifier();

            GetAttackPowerAfterCaps(rResult, 150.0);

            TorpedoAttackPower = (int)rResult;
        }

        void CheckCanParticipantInASW()
        {
            switch ((ShipType)r_Ship.Info.Type.ID)
            {
                case ShipType.Destroyer:
                case ShipType.LightCruiser:
                case ShipType.TorpedoCruiser:
                case ShipType.TrainingCruiser:
                case ShipType.FleetOiler:
                    CanParticipateInASW = (r_Ship.Status.ASW - r_Ship.EquipedEquipment.Sum(r => r.Info.ASW)) > 0;
                    break;

                case ShipType.AircraftCruiser:
                case ShipType.LightAircraftCarrier:
                case ShipType.AviationBattleship:
                case ShipType.SeaplaneCarrier:
                case ShipType.AmphibiousAssaultShip:
                    CanParticipateInASW= r_Ship.EquipedEquipment.Any(r => r.Info.IsPlane && r.Info.ASW > 0);
                    break;

                default:
                    CanParticipateInASW = false;
                    break;
            }
        }
        void CalculateASWAttackPower()
        {
            if (!CanParticipateInASW)
                return;

            var rSonerCount = 0;
            var rDepthChargerCount = 0;
            var rDepthChargerThrowerCount = 0;

            var rASWBase = r_Ship.Status.ASW;
            var rResult = .0;
            foreach (var rEquipment in r_Ship.EquipedEquipment)
            {
                rASWBase -= rEquipment.Info.ASW;

                switch (rEquipment.Info.Type)
                {
                    case EquipmentType.Soner:
                    case EquipmentType.LargeSoner:
                        rSonerCount++;
                        rResult += rEquipment.Info.ASW;
                        break;

                    case EquipmentType.DepthCharge:
                        switch (rEquipment.Info.ID)
                        {
                            case 226:
                            case 227:
                                rDepthChargerCount++;
                                break;

                            default:
                                rDepthChargerThrowerCount++;
                                break;
                        }
                        rResult += rEquipment.Info.ASW;
                        break;

                    case EquipmentType.CarrierBasedDiveBomber:
                    case EquipmentType.CarrierBasedTorpedoBomber:
                    case EquipmentType.SeaplaneBomber:
                    case EquipmentType.Autogyro:
                    case EquipmentType.ASAircraft:
                        rResult += rEquipment.Info.ASW;
                        break;
                }
            }
            rResult *= 1.5;

            rResult += Math.Sqrt(rASWBase) * 2 + GetASWAttackPowerBonusFromImprovedEquipment();

            if (DayBattleAttackMode == AttackMode.AerialStrike)
                rResult += 8.0;
            else
                rResult += 13.0;

            rResult *= GetHealthModifier();

            if (rSonerCount == 0)
            {
                if (rDepthChargerCount > 0 && rDepthChargerThrowerCount > 0)
                    rResult *= 1.1;
            }
            else
            {
                if (rDepthChargerCount > 0 && rDepthChargerThrowerCount > 0)
                    rResult *= 1.4375;
                else if (rDepthChargerCount > 0 || rDepthChargerThrowerCount > 0)
                    rResult *= 1.15;
            }

            GetAttackPowerAfterCaps(rResult, 100.0);

            ASWAttackPower = (int)rResult;
        }

        double GetHealthModifier()
        {
            switch (r_Ship.DamageState)
            {
                case ShipDamageState.ModeratelyDamaged:
                    return .7;

                case ShipDamageState.HeavilyDamaged:
                    return .4;

                default:
                    return 1.0;
            }
        }

        double GetDayBattleAttackPowerBonusFromImprovedEquipment()
        {
            var rResult = .0;

            foreach (var rEquipment in r_Ship.EquipedEquipment)
            {
                if (rEquipment.Level == 0)
                    continue;

                switch (rEquipment.Info.Type)
                {
                    case EquipmentType.LargeCaliberGun:
                    case EquipmentType.LargeCaliberGun2:
                        rResult += Math.Sqrt(rEquipment.Level) * 1.5;
                        break;

                    case EquipmentType.Soner:
                    case EquipmentType.DepthCharge:
                        rResult += Math.Sqrt(rEquipment.Level) * .75;
                        break;

                    case EquipmentType.SmallCaliberGun:
                    case EquipmentType.MediumCaliberGun:
                    case EquipmentType.SecondaryGun:
                    case EquipmentType.APShell:
                    case EquipmentType.AAFireDirector:
                    case EquipmentType.Searchlight:
                    case EquipmentType.AAGun:
                        rResult += Math.Sqrt(rEquipment.Level);
                        break;
                }
            }

            return rResult;
        }

        double GetAttackPowerBonusFromLightCruiserFitGun()
        {
            switch ((ShipType)r_Ship.Info.Type.ID)
            {
                case ShipType.LightCruiser:
                case ShipType.TorpedoCruiser:
                case ShipType.TrainingCruiser:
                    var rSingleGunCount = 0;
                    var rTwinGunCount = 0;

                    foreach (var rEquipment in r_Ship.EquipedEquipment)
                    {
                        switch (rEquipment.Info.ID)
                        {
                            // 14cm単装砲 / 14cm Single Gun Mount
                            case 4:
                            // 15.2cm単装砲 / 15.2cm Single Gun Mount
                            case 11:
                                rSingleGunCount++;
                                break;

                            // 15.2cm連装砲 / 15.2cm Twin Gun Mount
                            case 65:
                            // 14cm連装砲 / 14cm Twin Gun Mount
                            case 119:
                            // 15.2cm連装砲改 / 15.2cm Twin Gun Mount Kai
                            case 139:
                                rTwinGunCount++;
                                break;
                        }
                    }

                    return Math.Sqrt(rTwinGunCount) * 2.0 + Math.Sqrt(rSingleGunCount);

                default:
                    return .0;
            }
        }

        double GetAttackPowerAfterCaps(double rpPower, double rpCap)
        {
            if (rpPower > rpCap)
                rpPower = rpCap + Math.Sqrt(rpPower - rpCap);

            return rpPower;
        }

        double GetArtillerySpottingModifier()
        {
            if (DayBattleAttackMode == AttackMode.DoubleAttack)
                return 1.2;

            if (DayBattleAttackMode == AttackMode.CutIn)
                switch (DayBattleCutInType)
                {
                    case CutInType.DoubleMainGun:
                        return 1.5;

                    case CutInType.MainGunAndAPShell:
                        return 1.3;

                    case CutInType.MainGunAndRadar:
                        return 1.2;

                    case CutInType.MainGunAndSecondaryGun:
                        return 1.1;
                }

            return 1.0;
        }

        double GetNightBattleAttackPowerBonusFromImprovedEquipment()
        {
            var rResult = .0;

            foreach (var rEquipment in r_Ship.EquipedEquipment)
            {
                if (rEquipment.Level == 0)
                    continue;

                switch (rEquipment.Info.Type)
                {
                    case EquipmentType.SmallCaliberGun:
                    case EquipmentType.MediumCaliberGun:
                    case EquipmentType.LargeCaliberGun:
                    case EquipmentType.LargeCaliberGun2:
                    case EquipmentType.SecondaryGun:
                    case EquipmentType.Torpedo:
                    case EquipmentType.APShell:
                    case EquipmentType.Searchlight:
                    case EquipmentType.SubmarineTorpedo:
                    case EquipmentType.AAFireDirector:
                        rResult += Math.Sqrt(rEquipment.Level);
                        break;
                }
            }

            return rResult;
        }

        double GetNightSpecialAttackModifier()
        {
            if (NightBattleAttackMode == AttackMode.DoubleAttack)
                return 1.2;

            if (NightBattleAttackMode == AttackMode.CutIn)
                switch (NightBattleCutInType)
                {
                    case CutInType.TripleMainGun:
                        return 2.0;

                    case CutInType.DoubleMainGunAndSecondaryGun:
                        return 1.75;

                    case CutInType.DoubleTorpedo:
                        return 1.5;

                    case CutInType.Mixed:
                        return 1.3;
                }

            return 1.0;
        }

        double GetTorpedoAttackPowerBonusFromImprovedEquipment()
        {
            var rResult = .0;

            foreach (var rEquipment in r_Ship.EquipedEquipment)
            {
                if (rEquipment.Level == 0)
                    continue;

                switch (rEquipment.Info.Type)
                {
                    case EquipmentType.Torpedo:
                    case EquipmentType.SubmarineTorpedo:
                        rResult += Math.Sqrt(rEquipment.Level) * 1.2;
                        break;

                    case EquipmentType.AAGun:
                        rResult += Math.Sqrt(rEquipment.Level);
                        break;
                }
            }

            return rResult;
        }

        double GetASWAttackPowerBonusFromImprovedEquipment()
        {
            var rResult = .0;

            foreach (var rEquipment in r_Ship.EquipedEquipment)
            {
                if (rEquipment.Level == 0)
                    continue;

                switch (rEquipment.Info.Type)
                {
                    case EquipmentType.Soner:
                    case EquipmentType.DepthCharge:
                        rResult += Math.Sqrt(rEquipment.Level);
                        break;
                }
            }

            return rResult;
        }
    }
}
