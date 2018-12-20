using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawBattlePhase
    {
        IReadOnlyList<Attack> Attacks { get; }
    }
}
