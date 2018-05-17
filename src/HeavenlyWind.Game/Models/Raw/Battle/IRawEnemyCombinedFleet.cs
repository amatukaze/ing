namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public interface IRawEnemyCombinedFleet
    {
        int[] EnemyEscortShipTypeIDs { get; set; }

        int[] EnemyEscortShipLevels { get; set; }

        int[][] EnemyEscortShipEquipment { get; set; }

        int[][] EnemyEscortShipBaseStatus { get; set; }
    }
}
