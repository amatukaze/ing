using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AirForceSquadronRelocationCountdown : CountdownModelBase
    {
        AirForceGroup r_Group;
        AirForceSquadron r_Squadron;

        public AirForceSquadronRelocationCountdown(AirForceGroup rpGroup, AirForceSquadron rpSquadron)
        {
            r_Group = rpGroup;
            r_Squadron = rpSquadron;
        }

        public void Start()
        {
            TimeToComplete = DateTimeOffset.Now.AddMinutes(12.0);

            r_Group.UpdateRelocationCountdown();
        }

        public void Reset()
        {
            TimeToComplete = null;

            r_Group.UpdateRelocationCountdown();
        }

        protected override void TimeOut() => r_Squadron.RelocationComplete();
    }
}
