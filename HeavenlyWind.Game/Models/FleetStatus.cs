using System;
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

        int r_AA;
        public int AA
        {
            get { return r_AA; }
            private set
            {
                if (r_AA != value)
                {
                    r_AA = value;
                    OnPropertyChanged(nameof(AA));
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

            AA = r_Fleet.Ships.Sum(rpShip =>
                rpShip.Slots.Sum(rpSlot =>
                {
                    if (rpSlot.Equipment.Info.CanParticipateInFighterCombat)
                        return (int)(rpSlot.Equipment.Info.AA * Math.Sqrt(rpSlot.PlaneCount));
                    else
                        return 0;
                }));

            LoS.Update();
        }
    }
}
