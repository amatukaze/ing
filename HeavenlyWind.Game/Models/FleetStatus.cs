using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetStatus : ModelBase
    {
        static int[] r_CarrierBasedFighterFPBouns = { 0, 0, 2, 5, 9, 14, 14, 22 };
        static int[] r_SeaplaneBomberFPBouns = { 0, 0, 1, 1, 1, 3, 3, 6 };

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

        double r_MinFighterPower;
        public double MinFighterPower
        {
            get { return r_MinFighterPower; }
            private set
            {
                if (r_MinFighterPower != value)
                {
                    r_MinFighterPower = value;
                    OnPropertyChanged(nameof(MinFighterPower));
                }
            }
        }
        double r_MaxFighterPower;
        public double MaxFighterPower
        {
            get { return r_MaxFighterPower; }
            private set
            {
                if (r_MaxFighterPower != value)
                {
                    r_MaxFighterPower = value;
                    OnPropertyChanged(nameof(MaxFighterPower));
                }
            }
        }

        public FleetLoSStatus LoS { get; }

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

        internal FleetStatus(Fleet rpOwner)
        {
            r_Fleet = rpOwner;
            LoS = new FleetLoSStatus(rpOwner);
        }

        internal void Update()
        {
            TotalLevel = r_Fleet.Ships.Sum(r => r.Level);

            MinFighterPower = CalculateFighterPower(r => r == 0 ? .0 : (r - 1) * 15.0 + 10.0);
            MaxFighterPower = CalculateFighterPower(r => r == 9 ? .0 : (r - 1) * 15.0 + 24.0);

            LoS.Update();

            if (r_Fleet.Ships.Count == 0)
                Speed = null;
            else
            {
                var rSlowShip = 0;
                var rFastShip = 0;

                foreach (var rShip in r_Fleet.Ships)
                    switch (rShip.Info.Speed)
                    {
                        case ShipSpeed.Slow:
                            rSlowShip++;
                            break;

                        case ShipSpeed.Fast:
                            rFastShip++;
                            break;
                    }

                if (rSlowShip > 0 && rFastShip == 0)
                    Speed = FleetSpeed.Slow;
                else if (rFastShip > 0 && rSlowShip == 0)
                    Speed = FleetSpeed.Fast;
                else
                    Speed = FleetSpeed.Mixed;
            }
        }
        double CalculateFighterPower(Func<int, double> rpInternalBouns) =>
            r_Fleet.Ships.ExceptEvacuated().Sum(rpShip =>
                rpShip.Slots.Where(r => r.HasEquipment).Sum(rpSlot =>
                {
                    if (!rpSlot.Equipment.Info.CanParticipateInFighterCombat)
                        return .0;
                    else
                    {
                        var rResult = rpSlot.Equipment.Info.AA * Math.Sqrt(rpSlot.PlaneCount);

                        if (rpSlot.PlaneCount > 0)
                        {
                            var rProficiency = rpSlot.Equipment.Proficiency;

                            if (rpSlot.Equipment.Info.Type == EquipmentType.CarrierBasedFighter)
                                rResult += r_CarrierBasedFighterFPBouns[rProficiency];
                            else if (rpSlot.Equipment.Info.Type == EquipmentType.SeaplaneBomber)
                                rResult += r_SeaplaneBomberFPBouns[rProficiency];

                            rResult += Math.Sqrt(rpInternalBouns(rProficiency) / 10.0);
                        }

                        return rResult;
                    }
                }));
    }
}
