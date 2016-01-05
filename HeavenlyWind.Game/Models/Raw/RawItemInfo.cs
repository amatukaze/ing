using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawItemInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_usetype")]
        public int Type { get; set; }
        [JsonProperty("api_category")]
        public int Category { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_description")]
        public string[] Description { get; set; }

        //[JsonProperty("api_price")]
    }
}
