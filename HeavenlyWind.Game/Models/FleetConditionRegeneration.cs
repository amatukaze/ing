using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetConditionRegeneration : CountdownModelBase
    {
        Fleet r_Fleet;
        int r_LowestCondition;

        public event Action<Fleet> Recovered = delegate { };

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

            var rCeiling = Preference.Current.Game.FatigueCeiling;

            var rLowestCondition = r_Fleet.Ships.Min(r => r.Condition);
            if (rLowestCondition >= rCeiling)
            {
                TimeToComplete = null;
                return;
            }

            if (r_LowestCondition != rLowestCondition)
            {
                TimeToComplete = DateTimeOffset.Now.AddMinutes((int)Math.Ceiling((rCeiling - rLowestCondition) / 3.0) * 3);
                r_LowestCondition = rLowestCondition;
            }
        }

        internal void Reset() => TimeToComplete = null;

        protected override void TimeOut()
        {
            Reset();
            Recovered(r_Fleet);
        }
    }
}
