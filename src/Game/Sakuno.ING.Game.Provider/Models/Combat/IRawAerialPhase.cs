namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawAerialPhase : IRawBattlePhase
    {
        AirFightingResult FightingResult { get; }
        IRawAerialSide Ally { get; }
        IRawAerialSide Enemy { get; }
    }
}
