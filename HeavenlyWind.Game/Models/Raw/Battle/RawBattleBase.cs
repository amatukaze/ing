using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public abstract class RawBattleBase
    {
        public abstract int FleetID { get; set; }

        [JsonProperty("api_nowhps")]
        public int[] CurrentHPs { get; set; }

        [JsonProperty("api_ship_ke")]
        public int[] EnemyShipTypeIDs { get; set; }

        [JsonProperty("api_ship_lv")]
        public int[] EnemyShipLevels { get; set; }

        [JsonProperty("api_maxhps")]
        public int[] MaximumHPs { get; set; }

        [JsonProperty("api_eSlot")]
        public int[][] EnemyEquipment { get; set; }

        [JsonProperty("api_eKyouka")]
        public int[][] EnemyModernizationStatuses { get; set; }

        [JsonProperty("api_fParam")]
        public int[][] FriendBaseStatus { get; set; }

        [JsonProperty("api_eParam")]
        public int[][] EnemyBaseStatus { get; set; }

        [JsonProperty("api_combat_ration")]
        public int[] ShipsToConsumeCombatRation { get; set; }
        [JsonProperty("api_combat_ration_combined")]
        public int[] EscortShipsToConsumeCombatRation { get; set; }
    }
}
