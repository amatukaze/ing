using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetLoSStatus : ModelBase
    {
        Fleet r_Fleet;

        double r_Old25;
        public double Old25
        {
            get { return r_Old25; }
            private set
            {
                if (r_Old25 != value)
                {
                    r_Old25 = value;
                    OnPropertyChanged(nameof(Old25));
                }
            }
        }
        double r_Autumn25;
        public double Autumn25
        {
            get { return r_Autumn25; }
            private set
            {
                if (r_Autumn25 != value)
                {
                    r_Autumn25 = value;
                    OnPropertyChanged(nameof(Autumn25));
                }
            }
        }

        internal FleetLoSStatus(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update()
        {
            Old25 = CalclateWith25Old();
            Autumn25 = CalclateWith25Autumn();
        }

        double CalclateWith25Old()
        {
            var rShipLoS = .0;
            var rReconLoS = 0;
            var rRaderLoS = 0;

            foreach (var rShip in r_Fleet.Ships)
            {
                rShipLoS += rShip.Status.LoS;

                foreach (var rSlot in rShip.Slots.Where(r => r.HasEquipment))
                {
                    var rInfo = rSlot.Equipment.Info;

                    switch (rInfo.Icon)
                    {
                        case EquipmentIconType.CarrierBasedRecon:
                        case EquipmentIconType.Seaplane:
                            if (rSlot.PlaneCount > 0)
                            {
                                rReconLoS += rInfo.LoS;
                                rShipLoS -= rInfo.LoS;
                            }
                            break;

                        case EquipmentIconType.Rader:
                            rRaderLoS += rInfo.LoS;
                            rShipLoS -= rInfo.LoS;
                            break;
                    }
                }
            }

            rReconLoS *= 2;
            rShipLoS = Math.Sqrt(rShipLoS);

            return rShipLoS + rReconLoS + rRaderLoS;
        }

        double CalclateWith25Autumn()
        {
            var rShipLoS = .0;
            var rEquipmentLoS = .0;

            foreach (var rShip in r_Fleet.Ships)
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

            var rAdmiralLoS = Math.Floor((r_Fleet.Port.Admiral.Level + 4) / 5.0) * 5 * 0.6142467;

            return rShipLoS + rEquipmentLoS - 0;
        }
    }
}
