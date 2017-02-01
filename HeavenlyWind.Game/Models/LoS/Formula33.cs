using Sakuno.KanColle.Amatsukaze.Models;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.LoS
{
    class Formula33 : FleetLoSFormulaInfo
    {
        public override FleetLoSFormula Name { get; }

        int r_NodeFactor;

        public Formula33(FleetLoSFormula rpType)
        {
            Name = rpType;

            switch (rpType)
            {
                case FleetLoSFormula.Formula33:
                    r_NodeFactor = 1;
                    break;

                case FleetLoSFormula.Formula33Cn4:
                    r_NodeFactor = 4;
                    break;

                case FleetLoSFormula.Formula33Cn3:
                    r_NodeFactor = 3;
                    break;
            }
        }

        protected override double CalculateCore(Fleet rpFleet)
        {
            var rShipLoS = .0;
            var rEquipmentLoS = .0;
            var rEmptyShipSlotBonus = 12;

            foreach (var rShip in rpFleet.Ships)
            {
                if ((rShip.State & ShipState.Evacuated) != 0)
                    continue;

                rEmptyShipSlotBonus -= 2;

                var rShipLoSBase = rShip.Status.LoS;

                foreach (var rSlot in rShip.Slots)
                {
                    if (!rSlot.HasEquipment)
                        continue;

                    var rInfo = rSlot.Equipment.Info;
                    var rLoS = (double)rInfo.LoS;

                    rShipLoSBase -= rInfo.LoS;

                    var rLevel = rSlot.Equipment.Level;
                    if (rLevel > 0)
                        switch (rInfo.Type)
                        {
                            case EquipmentType.ReconSeaplane:
                                rLoS += Math.Sqrt(rLevel) * 1.2;
                                break;

                            case EquipmentType.SmallRadar:
                                rLoS += Math.Sqrt(rLevel) * 1.25;
                                break;

                            case EquipmentType.LargeRadar:
                                rLoS += Math.Sqrt(rLevel) * 1.4;
                                break;
                        }

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedTorpedoBomber:
                        case EquipmentType.JetPoweredAttackAircraft:
                            rEquipmentLoS += rLoS * .8;
                            break;

                        case EquipmentType.CarrierBasedRecon:
                        case EquipmentType.JetPoweredRecon:
                            rEquipmentLoS += rLoS;
                            break;

                        case EquipmentType.ReconSeaplane:
                            rEquipmentLoS += rLoS * 1.2;
                            break;

                        case EquipmentType.SeaplaneBomber:
                            rEquipmentLoS += rLoS * 1.1;
                            break;

                        case EquipmentType.SmallRadar:
                            rEquipmentLoS += rLoS * .6;
                            break;

                        case EquipmentType.LargeRadar:
                            rEquipmentLoS += rLoS * .6;
                            break;

                        default:
                            rEquipmentLoS += rLoS * .6;
                            break;
                    }
                }

                rShipLoS += Math.Sqrt(rShipLoSBase);
            }

            var rAdmiralLoS = Math.Ceiling(rpFleet.Port.Admiral.Level * .4);

            return rShipLoS + rEquipmentLoS * r_NodeFactor - rAdmiralLoS + rEmptyShipSlotBonus;
        }
    }
}
