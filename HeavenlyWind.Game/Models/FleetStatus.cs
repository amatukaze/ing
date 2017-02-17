using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetStatus : ModelBase
    {
        Fleet r_Fleet;

        int r_TotalLevel;
        public int TotalLevel
        {
            get { return r_TotalLevel; }
            private set
            {
                if (r_TotalLevel != value)
                {
                    r_TotalLevel = value;
                    OnPropertyChanged(nameof(TotalLevel));
                }
            }
        }

        public FleetFighterPowerStatus FighterPower { get; }

        public FleetLoSStatus[] LoS { get; }

        FleetSpeed? r_Speed;
        public FleetSpeed? Speed
        {
            get { return r_Speed; }
            private set
            {
                if (r_Speed != value)
                {
                    r_Speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }

        double r_TransportPoint;
        public double TransportPoint
        {
            get { return r_TransportPoint; }
            private set
            {
                if(r_TransportPoint!= value)
                {
                    r_TransportPoint = value;
                    OnPropertyChanged(nameof(TransportPoint));
                }
            }
        }

        internal FleetStatus(Fleet rpOwner)
        {
            r_Fleet = rpOwner;

            FighterPower = new FleetFighterPowerStatus(rpOwner);

            LoS = FleetLoSFormulaInfo.Formulas.Select(r => new FleetLoSStatus(rpOwner, r)).ToArray();
        }

        internal void Update()
        {
            TotalLevel = r_Fleet.Ships.Sum(r => r.Level);

            FighterPower.Update();

            foreach (var rLoS in LoS)
                rLoS.Update();

            UpdateFleetSpeed();

            CalculateTransportPoint();
        }

        void UpdateFleetSpeed()
        {
            if (r_Fleet.Ships.Count == 0)
                Speed = null;
            else
            {
                FleetSpeed rResult = 0;

                foreach (var rShip in r_Fleet.Ships)
                    switch (rShip.Speed)
                    {
                        case ShipSpeed.Slow:
                            rResult |= FleetSpeed.Slow;
                            break;

                        case ShipSpeed.Fast:
                        case ShipSpeed.FastPlus:
                        case ShipSpeed.UltraFast:
                            rResult |= FleetSpeed.Fast;
                            break;
                    }

                Speed = rResult;
            }
        }

        void CalculateTransportPoint()
        {
            var rResult = .0;

            foreach (var rShip in r_Fleet.Ships)
            {
                if (rShip.DamageState >= ShipDamageState.HeavilyDamaged || (rShip.State & ShipState.Evacuated) != 0)
                    continue;

                switch ((ShipType)rShip.Info.Type.ID)
                {
                    case ShipType.Destroyer:
                        rResult += 5.0;
                        break;

                    case ShipType.LightCruiser:
                        rResult += 2.0;
                        break;

                    case ShipType.AircraftCruiser:
                        rResult += 4.0;
                        break;

                    case ShipType.AviationBattleship:
                        rResult += 7.0;
                        break;

                    case ShipType.SeaplaneCarrier:
                        rResult += 9.0;
                        break;

                    case ShipType.AmphibiousAssaultShip:
                        rResult += 12.0;
                        break;

                    case ShipType.SubmarineTender:
                        rResult += 7.0;
                        break;

                    case ShipType.TrainingCruiser:
                        rResult += 6.0;
                        break;

                    case ShipType.FleetOiler:
                        rResult += 15.0;
                        break;
                }

                foreach (var rEquipment in rShip.EquipedEquipment)
                {
                    switch (rEquipment.Info.Type)
                    {
                        case EquipmentType.LandingCraft:
                            rResult += 8.0;
                            break;

                        case EquipmentType.SupplyTransportContainer:
                            rResult += 5.0;
                            break;

                        case EquipmentType.CombatRation:
                            rResult += 1.0;
                            break;

                        case EquipmentType.SpecialAmphibiousLandingCraft:
                            rResult += 2.0;
                            break;
                    }
                }

                TransportPoint = rResult;
            }
        }
    }
}
