using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawSupplyResult
    {
        [JsonProperty("api_ship")]
        public Ship[] Ships { get; set; }

        [JsonProperty("api_material")]
        public int[] Material { get; set; }

        [JsonProperty("api_use_bou")]
        public int BauxiteConsumption { get; set; }

        public class Ship
        {

            [JsonProperty("api_id")]
            public int ID { get; set; }

            [JsonProperty("api_fuel")]
            public int Fuel { get; set; }

            [JsonProperty("api_bull")]
            public int Bullet { get; set; }

            [JsonProperty("api_onslot")]
            public int[] Planes { get; set; }
        }
    }

}
