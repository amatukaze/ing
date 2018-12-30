using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawBattlePhase
    {
        IReadOnlyList<IRawAttack> Attacks { get; }
    }
}
