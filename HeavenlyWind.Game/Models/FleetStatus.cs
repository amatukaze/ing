using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetStatus : ModelBase
    {
        static int[] r_CarrierBasedFighterAABouns = { 0, 0, 2, 5, 9, 14, 14, 22 };
        static int[] r_SeaplaneBomberAABouns = { 0, 0, 1, 1, 1, 3, 3, 6 };

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

        double r_MinAA;
        public double MinAA
        {
            get { return r_MinAA; }
            private set
            {
                if (r_MinAA != value)
                {
                    r_MinAA = value;
                    OnPropertyChanged(nameof(MinAA));
                }
            }
        }
        double r_MaxAA;
        public double MaxAA
        {
            get { return r_MaxAA; }
            private set
            {
                if (r_MaxAA != value)
                {
                    r_MaxAA = value;
                    OnPropertyChanged(nameof(MaxAA));
                }
            }
        }

        public FleetLoSStatus LoS { get; }

        internal FleetStatus(Fleet rpOwner)
        {
            r_Fleet = rpOwner;
            LoS = new FleetLoSStatus(rpOwner);
        }

        internal void Update()
        {
            TotalLevel = r_Fleet.Ships.Sum(r => r.Level);

            MinAA = CalculateAA(r => r == 0 ? .0 : (r - 1) * 15.0 + 10.0);
            MaxAA = CalculateAA(r => r == 9 ? .0 : (r - 1) * 15.0 + 24.0);

            LoS.Update();
        }
        double CalculateAA(Func<int, double> rpInternalBouns) =>
            r_Fleet.Ships.Sum(rpShip =>
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
                                    rResult += r_CarrierBasedFighterAABouns[rProficiency];
                                else if (rpSlot.Equipment.Info.Type == EquipmentType.SeaplaneBomber)
                                    rResult += r_SeaplaneBomberAABouns[rProficiency];

                                rResult += Math.Sqrt(rpInternalBouns(rProficiency) / 10.0);
                            }

                            return rResult;
                        }
                    }));
    }
}
