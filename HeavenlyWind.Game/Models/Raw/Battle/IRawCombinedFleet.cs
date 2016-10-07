namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public interface IRawCombinedFleet
    {
        int[] EscortFleetCurrentHPs { get; set; }
        int[] EscortFleetMaximumHPs { get; set; }
    }
}