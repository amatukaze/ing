using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawCombinedFleetDay : RawCombinedFleetDayBase, IRawCombinedFleet
    {
        [JsonProperty("api_fParam")]
        public int[][] FriendEscortBaseStatus { get; set; }

        [JsonProperty("api_escape_idx")]
        public int[] MainFleetEscapedShipIndex { get; set; }
        [JsonProperty("api_escape_idx_combined")]
        public int[] EscortFleetEscapedShipIndex { get; set; }
    }
}
