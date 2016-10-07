using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawEnemyCombinedFleetDay : RawCombinedFleetDayBase, IRawEnemyCombinedFleet
    {
        [JsonProperty("api_ship_ke_combined")]
        public int[] EnemyEscortShipTypeIDs { get; set; }

        [JsonProperty("api_ship_lv_combined")]
        public int[] EnemyEscortShipLevels { get; set; }

        [JsonProperty("api_eSlot_combined")]
        public int[][] EnemyEscortShipEquipment { get; set; }

        [JsonProperty("api_eParam_combined")]
        public int[][] EnemyEscortShipBaseStatus { get; set; }
    }
}
