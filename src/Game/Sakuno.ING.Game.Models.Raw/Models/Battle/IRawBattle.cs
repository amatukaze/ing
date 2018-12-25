using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawBattle
    {
        Engagement Engagement { get; }
        IRawSide Ally { get; }
        IRawSide Enemy { get; }
        IReadOnlyList<IRawShipInBattle> NpcFleet { get; }
        IReadOnlyList<int> EscapedIndices { get; }

        IRawBattlePhase SheelingPhase1 { get; }
        IRawBattlePhase SheelingPhase2 { get; }
        IRawBattlePhase SheelingPhase3 { get; }
        IRawBattlePhase NightPhase { get; }
        IRawBattlePhase NightPhase1 { get; }
        IRawBattlePhase NightPhase2 { get; }
        IRawBattlePhase OpendingTorpedoPhase { get; }
        IRawBattlePhase ClosingTorpedoPhase { get; }
        IRawAerialPhase AerialPhase { get; }
        IRawAerialPhase AerialPhase2 { get; }
        IRawAerialPhase JetPhase { get; }
        IRawAerialPhase LandBaseJetPhase { get; }
        IReadOnlyList<IRawLandBaseAerialPhase> LandBasePhases { get; }
        SupportFireType? SupportFireType { get; }
        IReadOnlyList<ShipId?> SupportFleet { get; }
        IRawAerialPhase AerialSupportPhase { get; }
        IRawBattlePhase SupportPhase { get; }
        IRawNpcPhase NpcPhase { get; }
    }
}
