namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public interface IRawCombinedFleet
    {
        int[] EscortFleetCurrentHPs { get; set; }
        int[] EscortFleetMaximumHPs { get; set; }

        int[][] FriendEscortBaseStatus { get; set; }

        int[] MainFleetEscapedShipIndex { get; set; }
        int[] EscortFleetEscapedShipIndex { get; set; }
    }
}