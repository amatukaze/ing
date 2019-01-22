using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentRelocationResult
    {
        [JsonProperty("api_ship_data")]
        public RawShips Ships { get; set; }

        [JsonProperty("api_unset_list")]
        public RawUnequippedEquipment UnequippedEquipment { get; set; }

        [JsonProperty("api_bauxite")]
        public int? Bauxite { get; set; }

        public class RawShips
        {
            [JsonProperty("api_set_ship")]
            public RawShip Destination { get; set; }

            [JsonProperty("api_unset_ship")]
            public RawShip Origin { get; set; }
        }

        public class RawUnequippedEquipment
        {
            [JsonProperty("api_type3No")]
            public int Type { get; set; }

            [JsonProperty("api_slot_list")]
            public int[] IDs { get; set; }
        }
    }
}
