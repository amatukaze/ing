using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawConstructionResult
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_ship_id")]
        public int ShipID { get; set; }

        [JsonProperty("api_kdock")]
        public RawConstructionDock[] ConstructionDocks { get; set; }

        [JsonProperty("api_ship")]
        public RawShip Ship { get; set; }

        [JsonProperty("api_slotitem")]
        public RawEquipment[] Equipment { get; set; }
    }
}
