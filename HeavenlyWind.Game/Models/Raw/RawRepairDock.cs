using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawRepairDock : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_state")]
        public RepairDockState State { get; set; }

        [JsonProperty("api_ship_id")]
        public int ShipID { get; set; }

        [JsonProperty("api_complete_time")]
        public long CompleteTime { get; set; }

        [JsonProperty("api_item1")]
        public int FuelConsumption { get; set; }
        [JsonProperty("api_item2")]
        public int BulletConsumption { get; set; }
        [JsonProperty("api_item3")]
        public int SteelConsumption { get; set; }
        [JsonProperty("api_item4")]
        public int BauxiteConsumption { get; set; }
    }
}
