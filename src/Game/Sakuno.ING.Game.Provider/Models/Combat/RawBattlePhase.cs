using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public abstract class RawBattlePhase
    {
        protected RawBattlePhase(IEnumerable<RawAttack> attacks) => Attacks = attacks;

        public IEnumerable<RawAttack> Attacks { get; }
    }
}
