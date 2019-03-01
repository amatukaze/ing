namespace Sakuno.ING.Game.Models.Combat
{
    public interface IBattlePhaseBuilder
    {
        BattleParticipant MapAllyShip(int index);
        BattleParticipant MapEnemyShip(int index);
        AttackType MapType(int rawType);
    }
}
