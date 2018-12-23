using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawBattlePhase
    {
        IReadOnlyList<RawAttack> Attacks { get; }
    }
}
