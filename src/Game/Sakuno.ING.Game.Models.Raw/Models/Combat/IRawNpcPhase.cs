namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawNpcPhase : IRawBattlePhase
    {
        int? NpcFlareIndex { get; }
        int? EnemyFlareIndex { get; }
    }
}
