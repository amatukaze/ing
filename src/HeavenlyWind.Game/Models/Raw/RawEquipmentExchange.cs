using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentExchange
    {
        [JsonProperty("api_slot")]
        public int[] EquipmentIDs { get; set; }
        [JsonProperty("api_ship_data")]
        public RawShip Ship { get; set; }

        [JsonProperty("api_bauxite")]
        public int? Bauxite { get; set; }
    }
}
