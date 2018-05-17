using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawShipType : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_sortno")]
        public int SortNumber { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }
    }
}
