using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawBasic
    {
        [JsonProperty("api_member_id")]
        public int ID { get; set; }

        [JsonProperty("api_nickname")]
        public string Name { get; set; }

        [JsonProperty("api_level")]
        public int Level { get; set; }

        [JsonProperty("api_rank")]
        public int Rank { get; set; }

        [JsonProperty("api_experience")]
        public int Experience { get; set; }

        [JsonProperty("api_comment")]
        public string Comment { get; set; }

        [JsonProperty("api_max_chara")]
        public int MaxShipCount { get; set; }

        [JsonProperty("api_max_slotitem")]
        public int MaxEquipmentCount { get; set; }

        [JsonProperty("api_medals")]
        public int Medals { get; set; }
    }
}
