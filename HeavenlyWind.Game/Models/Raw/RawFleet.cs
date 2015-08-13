using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawFleet : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_mission")]
        public long[] Expedition { get; set; }

        [JsonProperty("api_ship")]
        public int[] Ship { get; set; }
    }
}
