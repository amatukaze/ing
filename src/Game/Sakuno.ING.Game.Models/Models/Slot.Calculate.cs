using System;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models
{
    public partial class Slot
    {
        private static readonly int[] afpBonus0 = { 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly int[] afpBonus1 = { 0, 0, 1, 1, 1, 3, 3, 6 };
        private static readonly int[] afpBonus2 = { 0, 0, 2, 5, 9, 14, 14, 22 };
        private static readonly int[] airProficiencyCore = { 0, 10, 25, 40, 55, 70, 85, 100, 121 };
        private void UpdateCalculations()
        {
            if (Equipment is null)
            {
                AirFightPower = default;
                EffectiveLoS = 0;
                return;
            }

            double losFactor = 0.6, losImprovementFactor = 0;
            int[] afpBonusTable = null;
            switch ((KnownEquipmentType)Equipment.Type.Id)
            {
                case KnownEquipmentType.FighterAircraft:
                case KnownEquipmentType.SeaplaneFighter:
                    afpBonusTable = afpBonus2;
                    break;
                case KnownEquipmentType.DiveBomber:
                case KnownEquipmentType.JetBomber:
                    afpBonusTable = afpBonus0;
                    break;
                case KnownEquipmentType.TorpedoBomber:
                    losFactor = 0.8;
                    afpBonusTable = afpBonus0;
                    break;
                case KnownEquipmentType.ReconnaissanceAircraft:
                    losFactor = 1;
                    break;
                case KnownEquipmentType.ReconnaissanceSeaplane:
                    losFactor = 1.2;
                    losImprovementFactor = 1.2;
                    break;
                case KnownEquipmentType.SeaplaneBomber:
                    losFactor = 1.1;
                    afpBonusTable = afpBonus1;
                    break;
                case KnownEquipmentType.SmallRadar:
                    losImprovementFactor = 1.25;
                    break;
                case KnownEquipmentType.LargeRadar:
                case KnownEquipmentType.VeryLargeRadar:
                    losImprovementFactor = 1.4;
                    break;
            }

            EffectiveLoS = losFactor * (Equipment.LineOfSight + ImprovementLevel * losImprovementFactor);
            if (afpBonusTable == null || Aircraft.Current == 0)
                AirFightPower = default;
            else
            {
                double afpRaw = Equipment.AntiAir * Math.Sqrt(Aircraft.Current);
                AirFightPower = new AirFightPower(afpRaw,
                    afpRaw + Math.Sqrt(airProficiencyCore[AirProficiency] / 10.0) + afpBonusTable[AirProficiency],
                    afpRaw + Math.Sqrt((airProficiencyCore[AirProficiency + 1] - 1) / 10.0) + afpBonusTable[AirProficiency]);
            }
        }
    }
}
