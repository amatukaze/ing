using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.LoS
{
    class AutumnFormula : FleetLoSFormulaInfo
    {
        public override FleetLoSFormula Name => FleetLoSFormula.Autumn;

        protected override double CalculateCore(Fleet rpFleet)
        {
            var rShipLoS = .0;
            var rEquipmentLoS = .0;

            foreach (var rShip in rpFleet.Ships.ExceptEvacuated())
            {
                var rShipLoSBase = (double)rShip.Status.LoS;

                foreach (var rSlot in rShip.Slots.Where(r => r.HasEquipment))
                {
                    var rInfo = rSlot.Equipment.Info;
                    var rLoS = (double)rInfo.LoS;
                    rShipLoSBase -= rLoS;

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedDiveBomber:
                            rEquipmentLoS += rLoS * 1.0376255;
                            break;

                        case EquipmentType.CarrierBasedTorpedoBomber:
                            rEquipmentLoS += rLoS * 1.3677954;
                            break;

                        case EquipmentType.CarrierBasedRecon:
                            rEquipmentLoS += rLoS * 1.6592780;
                            break;

                        case EquipmentType.ReconSeaplane:
                            rEquipmentLoS += rLoS * 2.0;
                            break;

                        case EquipmentType.SeaplaneBomber:
                            rEquipmentLoS += rLoS * 1.7787282;
                            break;

                        case EquipmentType.SmallRadar:
                            rEquipmentLoS += rLoS * 1.0045358;
                            break;

                        case EquipmentType.LargeRadar:
                            rEquipmentLoS += rLoS * 0.9906638;
                            break;

                        case EquipmentType.Searchlight:
                            rEquipmentLoS += rLoS * 0.9067950;
                            break;
                    }
                }

                rShipLoS += Math.Sqrt(rShipLoSBase) * 1.6841056;
            }

            var rAdmiralLoS = Math.Ceiling(rpFleet.Port.Admiral.Level / 5.0) * 5.0 * 0.6142467;

            return rShipLoS + rEquipmentLoS - rAdmiralLoS;
        }
    }
}
