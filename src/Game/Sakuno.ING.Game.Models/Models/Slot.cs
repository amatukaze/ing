using System;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models
{
    public abstract partial class Slot : BindableObject
    {
        public abstract Equipment Equipment { get; }

        private static readonly int[] afpBonus0 = { 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly int[] afpBonus1 = { 0, 0, 1, 1, 1, 3, 3, 6 };
        private static readonly int[] afpBonus2 = { 0, 0, 2, 5, 9, 14, 14, 22 };
        private static readonly int[] airProficiencyCore = { 0, 10, 25, 40, 55, 70, 85, 100, 121 };

        internal void DoCalculations()
        {
            using (EnterBatchNotifyScope())
            {
                if (Equipment?.Info?.Type is null)
                {
                    AirFightPower = default;
                    EffectiveLoS = 0;
                    return;
                }

                var id = (KnownEquipmentType)Equipment.Info.Type.Id;

                double losFactor = id switch
                {
                    KnownEquipmentType.TorpedoBomber => 0.8,
                    KnownEquipmentType.ReconnaissanceAircraft => 1,
                    KnownEquipmentType.ReconnaissanceSeaplane => 1.2,
                    KnownEquipmentType.SeaplaneBomber => 1.1,
                    _ => 0.6,
                };
                double losImprovementFactor = id switch
                {
                    KnownEquipmentType.ReconnaissanceSeaplane => 1.2,
                    KnownEquipmentType.SmallRadar => 1.25,
                    KnownEquipmentType.LargeRadar => 1.4,
                    KnownEquipmentType.VeryLargeRadar => 1.4,
                    _ => 0
                };
                int[] afpBonusTable = id switch
                {
                    KnownEquipmentType.DiveBomber => afpBonus0,
                    KnownEquipmentType.JetBomber => afpBonus0,
                    KnownEquipmentType.TorpedoBomber => afpBonus0,
                    KnownEquipmentType.SeaplaneBomber => afpBonus1,
                    KnownEquipmentType.FighterAircraft => afpBonus2,
                    KnownEquipmentType.SeaplaneFighter => afpBonus2,
                    _ => null
                };

                EffectiveLoS = losFactor * (Equipment.Info.LineOfSight + Equipment.ImprovementLevel * losImprovementFactor);

                if (afpBonusTable == null || Aircraft.Current == 0)
                    AirFightPower = default;
                else
                {
                    double afpImprovementFactor = id switch
                    {
                        KnownEquipmentType.FighterAircraft => 0.2,
                        KnownEquipmentType.SeaplaneFighter => 0.2,
                        KnownEquipmentType.LandBasedFighter => 0.2,
                        KnownEquipmentType.DiveBomber => 0.25,
                        _ => 0
                    };
                    double afpRaw = (Equipment.Info.AntiAir + afpImprovementFactor * Equipment.ImprovementLevel) * Math.Sqrt(Aircraft.Current);
                    AirFightPower = new AirFightPower(afpRaw,
                        afpRaw + Math.Sqrt(airProficiencyCore[Equipment.AirProficiency] / 10.0) + afpBonusTable[Equipment.AirProficiency],
                                afpRaw + Math.Sqrt((airProficiencyCore[Equipment.AirProficiency + 1] - 1) / 10.0) + afpBonusTable[Equipment.AirProficiency]);
                }
            }
        }
    }
}
