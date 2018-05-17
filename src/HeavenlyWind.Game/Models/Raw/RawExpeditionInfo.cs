using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawExpeditionInfo : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_maparea_id")]
        public int MapAreaID { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_details")]
        public string Description { get; set; }

        [JsonProperty("api_time")]
        public int Time { get; set; }

        [JsonProperty("api_difficulty")]
        public int Difficulty { get; set; }

        [JsonProperty("api_use_fuel")]
        public double FuelConsumption { get; set; }
        [JsonProperty("api_use_bull")]
        public double BulletConsumption { get; set; }

        [JsonProperty("api_win_item1")]
        public int[] RewardItem1 { get; set; }
        [JsonProperty("api_win_item2")]
        public int[] RewardItem2 { get; set; }

        [JsonProperty("api_return_flag")]
        public bool CanReturn { get; set; }
    }
}
