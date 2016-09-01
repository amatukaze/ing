using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetAnchorageRepair : CountdownModelBase
    {
        Fleet r_Fleet;

        internal List<Tuple<Ship, int>> RepairingShips { get; set; }

        public event Action InterruptionNotification = () => { };

        internal FleetAnchorageRepair(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update(IEnumerable<Ship> rpShipsToBeRepaired)
        {
            var rShips = rpShipsToBeRepaired.Select(r => Tuple.Create(r, r.HP.Current)).ToList();
            var rReset = false;

            if (RepairingShips == null || !RepairingShips.Select(r => r.Item1).SequenceEqual(rpShipsToBeRepaired))
            {
                if (RepairingShips != null)
                    foreach (var rShip in RepairingShips.Select(r => r.Item1).Except(rpShipsToBeRepaired))
                        rShip.UpdateAnchorageRepairStatus(false);

                rReset = true;
            }
            else
            {
                for (var i = 0; i < RepairingShips.Count; i++)
                    if (RepairingShips[i].Item2 != rShips[i].Item2)
                    {
                        rReset = true;
                        break;
                    }
            }

            if (rReset)
            {
                foreach (var rShip in rShips)
                {
                    rShip.Item1.UpdateAnchorageRepairStatus(true);
                    rShip.Item1.AnchorageRepairStatus.Update();
                }

                RepairingShips = rShips;
                Reset();
            }
        }

        internal void Reset() => TimeToComplete = DateTimeOffset.Now.AddMinutes(20.0);
        internal void Stop()
        {
            foreach (var rShip in RepairingShips)
                rShip.Item1.UpdateAnchorageRepairStatus(false);

            TimeToComplete = null;
            RepairingShips = null;
        }

        internal void RemoveShipIfExists(Ship rpShip)
        {
            if (RepairingShips == null)
                return;

            var rIndex = RepairingShips.FindIndex(r => r.Item1 == rpShip);
            if (rIndex != -1)
            {
                rpShip.UpdateAnchorageRepairStatus(false);
                RepairingShips.RemoveAt(rIndex);
            }
        }

        protected override void TimeOut() => InterruptionNotification();
    }
}
