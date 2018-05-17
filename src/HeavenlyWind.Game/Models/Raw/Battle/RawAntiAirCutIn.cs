using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawAntiAirCutIn
    {
        [JsonProperty("api_idx")]
        public int TriggererIndex { get; set; }

        [JsonProperty("api_kind")]
        public int TypeID { get; set; }

        [JsonProperty("api_use_items")]
        public int[] EquipmentIDs { get; set; }
    }
}
