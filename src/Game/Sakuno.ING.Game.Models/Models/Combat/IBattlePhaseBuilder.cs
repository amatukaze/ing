namespace Sakuno.ING.Game.Models.Combat
{
    public interface IBattlePhaseBuilder
    {
        BattleParticipant MapShip(int index, bool isEnemy);
        AttackType MapType(int rawType);
    }
}
