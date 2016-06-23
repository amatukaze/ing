using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.LoS
{
    class Formula33 : FleetLoSFormulaInfo
    {
        public override FleetLoSFormula Name => FleetLoSFormula.Formula33;

        protected override double CalculateCore(Fleet rpFleet)
        {
            var rShipLoS = .0;
            var rShipCount = 0;
            var rEquipmentLoS = .0;

            foreach (var rShip in rpFleet.Ships.ExceptEvacuated())
            {
                var rShipLoSBase = rShip.Status.LoS;
                rShipCount++;

                foreach (var rSlot in rShip.Slots.Where(r => r.HasEquipment))
                {
                    var rInfo = rSlot.Equipment.Info;
                    var rLoS = (double)rInfo.LoS;
                    rShipLoSBase -= rInfo.LoS;

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedTorpedoBomber:
                            rEquipmentLoS += rLoS * .8;
                            break;

                        case EquipmentType.CarrierBasedRecon:
                            rEquipmentLoS += rLoS;
                            break;

                        case EquipmentType.ReconSeaplane:
                            rEquipmentLoS += (rLoS + Math.Sqrt(rSlot.Equipment.Level) * 1.2) * 1.2;
                            break;

                        case EquipmentType.SeaplaneBomber:
                            rEquipmentLoS += rLoS * 1.1;
                            break;

                        case EquipmentType.SmallRadar:
                        case EquipmentType.LargeRadar:
                            rEquipmentLoS += (rLoS + Math.Sqrt(rSlot.Equipment.Level) * 1.25) * .6;
                            break;

                        default:
                            rEquipmentLoS += rLoS * .6;
                            break;
                    }
                }

                rShipLoS += Math.Sqrt(rShipLoSBase);
            }

            var rAdmiralLoS = Math.Ceiling(rpFleet.Port.Admiral.Level * .4);
            var rEmptyShipslotBonus = (6 - rShipCount) * 2;

            return rShipLoS + rEquipmentLoS - rAdmiralLoS + rEmptyShipslotBonus;
        }
    }
}
