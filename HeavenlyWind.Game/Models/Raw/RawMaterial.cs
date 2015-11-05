using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawMaterial
    {
        [JsonProperty("api_id")]
        public MaterialType Type { get; set; }

        [JsonProperty("api_value")]
        public int Amount { get; set; }
    }
}
