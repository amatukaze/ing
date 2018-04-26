using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
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
        [JsonProperty("api_details"), JsonConverter(typeof(HtmlNewLineEater))]
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

        [JsonProperty("api_win_item1"), JsonConverter(typeof(ItemRecordConverter))]
        public ItemRecord RewardItem1;

        [JsonProperty("api_win_item2"), JsonConverter(typeof(ItemRecordConverter))]
        public ItemRecord RewardItem2;

        public IReadOnlyList<ItemRecord> RewardItems
        {
            get
            {
                if (RewardItem1.ItemId == 0)
                    return Array.Empty<ItemRecord>();
                if (RewardItem2.ItemId == 0)
                    return new[] { RewardItem1 };
                return new[] { RewardItem1, RewardItem2 };
            }
        }

        [JsonProperty("api_return_flag")]
        public bool CanRecall { get; set; }
    }
}
