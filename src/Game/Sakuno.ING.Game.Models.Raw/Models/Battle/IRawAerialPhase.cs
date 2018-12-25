namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawAerialPhase : IRawBattlePhase
    {
        AirFightingResult FightingResult { get; }
        IRawAerialSide Ally { get; }
        IRawAerialSide Enemy { get; }
    }
}
