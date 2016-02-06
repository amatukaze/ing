using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ShipLocking : IID
    {
        [JsonProperty("id")]
        public int ID { get; internal set; }

        [JsonProperty("color")]
        public string Color { get; internal set; }
    }
}
