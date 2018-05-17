using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawConstructionDock : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_state")]
        public ConstructionDockState State { get; set; }

        [JsonProperty("api_created_ship_id")]
        public int ShipID { get; set; }

        [JsonProperty("api_complete_time")]
        public long TimeToComplete { get; set; }

        [JsonProperty("api_item1")]
        public int FuelConsumption { get; set; }
        [JsonProperty("api_item2")]
        public int BulletConsumption { get; set; }
        [JsonProperty("api_item3")]
        public int SteelConsumption { get; set; }
        [JsonProperty("api_item4")]
        public int BauxiteConsumption { get; set; }
        [JsonProperty("api_item5")]
        public int DevelopmentMaterialConsumption { get; set; }

    }
}
