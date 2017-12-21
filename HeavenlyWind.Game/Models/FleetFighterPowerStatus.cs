using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetFighterPowerStatus : ModelBase
    {
        static int[] r_FighterFPBouns = { 0, 0, 2, 5, 9, 14, 14, 22 };
        static int[] r_SeaplaneBomberFPBouns = { 0, 0, 1, 1, 1, 3, 3, 6 };
        static int[] r_InternalFPBonus = { 10, 25, 40, 55, 70, 85, 100, 120 };

        Fleet r_Fleet;

        public IDictionary<FleetFighterPowerFormula, double> Formulas { get; private set; }

        internal FleetFighterPowerStatus(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update()
        {
            var rFighterPowerWithoutBonus = 0;
            var rFighterPower = 0;

            foreach (var rShip in r_Fleet.Ships)
            {
                if ((rShip.State & ShipState.Evacuated) != 0)
                    continue;

                foreach (var rSlot in rShip.Slots)
                {
                    if (!rSlot.HasEquipment)
                        continue;

                    var rEquipment = rSlot.Equipment;
                    var rInfo = rEquipment.Info;

                    if (!rInfo.CanParticipateInFighterCombat)
                        continue;

                    var rSquareRootOfPlaneCount = Math.Sqrt(rSlot.PlaneCount);
                    var rResult = rInfo.AA * rSquareRootOfPlaneCount;

                    rFighterPowerWithoutBonus += (int)rResult;

                    switch (rInfo.Type)
                    {
                        case EquipmentType.CarrierBasedFighter:
                        case EquipmentType.SeaplaneFighter:
                            rResult += rEquipment.Level * .2 * rSquareRootOfPlaneCount;
                            break;

                        case EquipmentType.CarrierBasedDiveBomber:
                            rResult += rEquipment.Level * .25 * rSquareRootOfPlaneCount;
                            break;
                    }

                    if (rSlot.PlaneCount > 0)
                    {
                        var rProficiency = rEquipment.Proficiency;

                        switch (rInfo.Type)
                        {
                            case EquipmentType.CarrierBasedFighter:
                            case EquipmentType.SeaplaneFighter:
                            case EquipmentType.JetPoweredFighter:
                                rResult += r_FighterFPBouns[rProficiency];
                                break;

                            case EquipmentType.SeaplaneBomber:
                                rResult += r_SeaplaneBomberFPBouns[rProficiency];
                                break;
                        }

                        rResult += Math.Sqrt(r_InternalFPBonus[rProficiency] * .1);
                    }
                    rFighterPower += (int)rResult;
                }
            }

            Formulas = new ListDictionary<FleetFighterPowerFormula, double>()
            {
                { FleetFighterPowerFormula.WithoutBonus, rFighterPowerWithoutBonus },
                { FleetFighterPowerFormula.WithBonus, rFighterPower },
            };
            OnPropertyChanged(nameof(Formulas));
        }
    }
}
