using System;
using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class ExpeditionInfoJson : IRawExpeditionInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_disp_no")]
        public string DisplayId { get; set; }
        [JsonProperty("api_maparea_id")]
        public int MapAreaId { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_details", ItemConverterType = typeof(HtmlNewLineEater))]
        public string Description { get; set; }
        public int api_time;
        public TimeSpan Duration => TimeSpan.FromMinutes(api_time);

        [JsonProperty("api_deck_num")]
        public int RequiredShipCount { get; set; }
        [JsonProperty("api_difficulty")]
        public int Difficulty { get; set; }
        [JsonProperty("api_use_fuel")]
        public double FuelConsumption { get; set; }
        [JsonProperty("api_use_bull")]
        public double BulletConsumption { get; set; }

        public int[] api_win_item1;
        public int[] api_win_item2;
        public int RewardItem1Id => api_win_item1.ElementAtOrDefault(0);
        public int RewardItem1Count => api_win_item1.ElementAtOrDefault(1);
        public int RewardItem2Id => api_win_item2.ElementAtOrDefault(0);
        public int RewardItem2Count => api_win_item2.ElementAtOrDefault(1);

        [JsonProperty("api_return_flag")]
        public bool CanRecall { get; set; }
    }
}
