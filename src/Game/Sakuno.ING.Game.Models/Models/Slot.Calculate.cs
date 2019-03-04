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

            (double losFactor, double losImprovementFactor, int[] afpBonusTable) = (KnownEquipmentType)Equipment.Type.Id switch
            {
                KnownEquipmentType.FighterAircraft => (0.6, 0d, afpBonus2),
                KnownEquipmentType.SeaplaneFighter => (0.6, 0d, afpBonus2),
                KnownEquipmentType.DiveBomber => (0.6, 0d, afpBonus0),
                KnownEquipmentType.JetBomber => (0.6, 0d, afpBonus0),
                KnownEquipmentType.TorpedoBomber => (0.8, 0d, afpBonus0),
                KnownEquipmentType.ReconnaissanceAircraft => (1, 0d, null),
                KnownEquipmentType.ReconnaissanceSeaplane => (1.2, 1.2, null),
                KnownEquipmentType.SeaplaneBomber => (1.1, 0d, afpBonus1),
                KnownEquipmentType.SmallRadar => (0.6, 1.25, null),
                KnownEquipmentType.LargeRadar => (0.6, 1.4, null),
                KnownEquipmentType.VeryLargeRadar => (0.6, 1.4, null),
                _ => (0.6, 0d, null)
            };

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
