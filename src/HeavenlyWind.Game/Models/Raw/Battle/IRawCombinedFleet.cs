namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public interface IRawCombinedFleet
    {
        int[] FriendEscortCurrentHPs { get; set; }
        int[] FriendEscortMaximumHPs { get; set; }
        int[] EnemyEscortCurrentHPs { get; set; }
        int[] EnemyEscortMaximumHPs { get; set; }
    }
}