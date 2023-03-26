namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public interface IRawCombinedFleet
    {
        int[] FriendEscortCurrentHPs { get; }
        int[] FriendEscortMaximumHPs { get; }
        int[] EnemyEscortCurrentHPs { get; }
        int[] EnemyEscortMaximumHPs { get; }
    }
}