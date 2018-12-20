using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawBattle
    {
        BattleKind Kind { get; }
        Engagement Engagement { get; }
        IReadOnlyList<IRawBattlePhase> Phases { get; }
        Formation AllyFormation { get; }
        IReadOnlyList<IRawShipInBattle> AllyShips { get; }
        Formation EnemyFormation { get; }
        IReadOnlyList<IRawShipInBattle> EnemyShips { get; }
        IReadOnlyList<IRawShipInBattle> NpcShips { get; }
    }
}
