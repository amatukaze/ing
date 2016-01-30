using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetConditionRegeneration : CountdownModelBase
    {
        Fleet r_Fleet;
        int r_LowestCondition;

        internal FleetConditionRegeneration(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update()
        {
            if (r_Fleet.Ships.Count == 0)
            {
                TimeToComplete = null;
                return;
            }

            var rLowestCondition = r_Fleet.Ships.Min(r => r.Condition);
            if (rLowestCondition >= 49)
            {
                TimeToComplete = null;
                return;
            }

            if (r_LowestCondition != rLowestCondition)
            {
                TimeToComplete = DateTimeOffset.Now.AddMinutes((int)Math.Ceiling((49 - rLowestCondition) / 3.0) * 3);
                r_LowestCondition = rLowestCondition;
            }
        }

        protected override void TimeOut()
        {

        }
    }
}
