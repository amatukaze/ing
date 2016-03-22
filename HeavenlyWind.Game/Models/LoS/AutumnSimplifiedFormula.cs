using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.LoS
{
    class AutumnSimplifiedFormula : FleetLoSFormulaInfo
    {
        public override FleetLoSFormula Name => FleetLoSFormula.AutumnSimplified;

        protected override double CalculateCore(Fleet rpFleet)
        {
            var rResult = .0;

            foreach (var rShip in rpFleet.Ships.ExceptEvacuated())
            {
                var rShipLoS = rShip.Status.LoS;
                var rEquipmentLoS = .0;

                foreach (var rSlot in rShip.Slots.Where(r => r.HasEquipment))
                {
                    var rInfo = rSlot.Equipment.Info;
                    var rLoS = (double)rInfo.LoS;
                    rShipLoS -= rInfo.LoS;

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedDiveBomber:
                            rEquipmentLoS += rLoS * .6;
                            break;

                        case EquipmentType.CarrierBasedTorpedoBomber:
                            rEquipmentLoS += rLoS * .8;
                            break;

                        case EquipmentType.CarrierBasedRecon:
                            rEquipmentLoS += rLoS;
                            break;

                        case EquipmentType.ReconSeaplane:
                            rEquipmentLoS += rLoS * 1.2;
                            break;

                        case EquipmentType.SeaplaneBomber:
                            rEquipmentLoS += rLoS;
                            break;

                        case EquipmentType.SmallRadar:
                        case EquipmentType.LargeRadar:
                            rEquipmentLoS += rLoS * .6;
                            break;

                        default:
                            rEquipmentLoS += rLoS * .5;
                            break;
                    }
                }

                rResult += Math.Floor(Math.Sqrt(rShipLoS) + rEquipmentLoS);
            }

            rResult -= Math.Floor(rpFleet.Port.Admiral.Level * .4);

            return rResult;
        }
    }
}
