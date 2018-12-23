using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawBattle
    {
        Engagement Engagement { get; }
        RawSide Ally { get; }
        RawSide Enemy { get; }
        IReadOnlyList<IRawShipInBattle> NpcFleet { get; }
        IReadOnlyList<int> EscapedIndices { get; }
    }
}
