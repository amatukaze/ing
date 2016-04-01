using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.LoS
{
    class OldFormula : FleetLoSFormulaInfo
    {
        public override FleetLoSFormula Name => FleetLoSFormula.Old;

        protected override double CalculateCore(Fleet rpFleet)
        {
            var rShipLoS = 0;
            var rReconLoS = 0;
            var rRaderLoS = 0;

            foreach (var rShip in rpFleet.Ships.ExceptEvacuated())
            {
                rShipLoS += rShip.Status.LoS;

                foreach (var rSlot in rShip.Slots.Where(r => r.HasEquipment))
                {
                    var rInfo = rSlot.Equipment.Info;

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedRecon:
                        case EquipmentType.ReconSeaplane:
                        case EquipmentType.SeaplaneBomber:
                            if (rSlot.PlaneCount > 0)
                            {
                                rReconLoS += rInfo.LoS;
                                rShipLoS -= rInfo.LoS;
                            }
                            break;

                        case EquipmentType.SmallRadar:
                        case EquipmentType.LargeRadar:
                            rRaderLoS += rInfo.LoS;
                            rShipLoS -= rInfo.LoS;
                            break;
                    }
                }
            }

            rReconLoS *= 2;

            return Math.Sqrt(rShipLoS) + rReconLoS + rRaderLoS;
        }
    }
}
