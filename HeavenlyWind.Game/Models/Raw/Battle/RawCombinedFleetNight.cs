using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawCombinedFleetNight : RawNight, IRawCombinedFleet
    {
        [JsonProperty("api_nowhps_combined")]
        public int[] EscortFleetCurrentHPs { get; set; }
        [JsonProperty("api_maxhps_combined")]
        public int[] EscortFleetMaximumHPs { get; set; }

        [JsonProperty("api_fParam")]
        public int[][] FriendEscortBaseStatus { get; set; }

        [JsonProperty("api_escape_idx")]
        public int[] MainFleetEscapedShipIndex { get; set; }
        [JsonProperty("api_escape_idx_combined")]
        public int[] EscortFleetEscapedShipIndex { get; set; }
    }
}
