using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawModernization
    {
        [JsonProperty("api_powerup_flag")]
        public bool Success { get; set; }

        [JsonProperty("api_ship")]
        public RawShip Ship { get; set; }
        [JsonProperty("api_deck")]
        public RawFleet[] Fleets { get; set; }
    }
}
