using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawBattle
    {
        Engagement Engagement { get; }
        ref RawSide Ally { get; }
        ref RawSide Enemy { get; }
        IReadOnlyList<IRawShipInBattle> NpcFleet { get; }
        IReadOnlyList<int> EscapedIndices { get; }

        IRawBattlePhase SheelingPhase1 { get; }
        IRawBattlePhase SheelingPhase2 { get; }
        IRawBattlePhase SheelingPhase3 { get; }
        IRawBattlePhase NightPhase { get; }
        IRawBattlePhase NightPhase1 { get; }
        IRawBattlePhase NightPhase2 { get; }
    }
}
