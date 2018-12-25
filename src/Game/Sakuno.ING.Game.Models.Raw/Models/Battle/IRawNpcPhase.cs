namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawNpcPhase : IRawBattlePhase
    {
        int? NpcFlareIndex { get; }
        int? EnemyFlareIndex { get; }
    }
}
