using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawShipsAndFleets
    {
        [JsonProperty("api_ship_data")]
        public RawShip[] Ships { get; set; }

        [JsonProperty("api_deck_data")]
        public RawFleet[] Fleets { get; set; }
    }
}
