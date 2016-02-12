namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public interface IAerialCombat
    {
        RawAerialCombatPhase AerialCombat { get; }
    }
    public interface IAerialCombatSecondRound : IAerialCombat
    {
        RawAerialCombatPhase AerialCombatSecondRound { get; }
    }
}
