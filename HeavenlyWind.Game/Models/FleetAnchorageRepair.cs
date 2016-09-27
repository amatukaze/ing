using Sakuno.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetAnchorageRepair : CountdownModelBase
    {
        Fleet r_Fleet;

        ListDictionary<Ship, int> r_Snapshots;

        public event Action InterruptionNotification;

        internal FleetAnchorageRepair(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update(IEnumerable<Ship> rpShips)
        {
            var rSnapshots = new ListDictionary<Ship, int>();
            foreach (var rShip in rpShips)
                rSnapshots.Add(rShip, rShip.HP.Current);

            if (r_Snapshots == null || !r_Snapshots.Keys.SequenceEqual(rpShips) || !r_Snapshots.Values.SequenceEqual(rpShips.Select(r => r.HP.Current)))
            {
                if (r_Snapshots != null)
                    foreach (var rShip in r_Snapshots.Keys.Except(rpShips))
                        rShip.UpdateAnchorageRepairStatus(false);

                foreach (var rShip in rSnapshots.Keys)
                {
                    rShip.UpdateAnchorageRepairStatus(true);
                    rShip.AnchorageRepairStatus.Update();
                }

                r_Snapshots = rSnapshots;
                Reset();
            }
        }

        internal void Reset() => TimeToComplete = DateTimeOffset.Now.AddMinutes(20.0);
        internal void Stop()
        {
            foreach (var rShip in r_Snapshots.Keys)
                rShip.UpdateAnchorageRepairStatus(false);

            TimeToComplete = null;
            r_Snapshots = null;
        }

        internal bool IsBeingAnchorageRepair(Ship rpShip) => r_Snapshots != null && r_Snapshots.ContainsKey(rpShip);
        internal void RemoveShipIfExists(Ship rpShip)
        {
            if (r_Snapshots == null)
                return;

            r_Snapshots.Remove(rpShip);
            rpShip.UpdateAnchorageRepairStatus(false);
        }

        protected override void TimeOut() => InterruptionNotification?.Invoke();
    }
}
