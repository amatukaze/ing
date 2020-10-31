using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawExpeditionInfo
    {
        [JsonPropertyName("api_id")]
        public ExpeditionId Id { get; set; }
        [JsonPropertyName("api_disp_no")]
        public string DisplayId { get; set; }
        [JsonPropertyName("api_maparea_id")]
        public MapAreaId MapAreaId { get; set; }

        [JsonPropertyName("api_name")]
        public string Name { get; set; }
        [JsonPropertyName("api_details")]
        public string Description { get; set; }
        [JsonPropertyName("api_time")]
        [JsonConverter(typeof(TimeSpanInMinuteConverter))]
        public TimeSpan Duration { get; set; }

        [JsonPropertyName("api_deck_num")]
        public int MinShipCount { get; set; }
        [JsonPropertyName("api_difficulty")]
        public int Difficulty { get; set; }
        [JsonPropertyName("api_use_fuel")]
        public double FuelConsumptionPercentage { get; set; }
        [JsonPropertyName("api_use_bull")]
        public double BulletConsumptionPercentage { get; set; }

        [JsonPropertyName("api_win_item1")]
        public ExpeditionUseItemReward? UseItemReward1 { get; set; }
        [JsonPropertyName("api_win_item2")]
        public ExpeditionUseItemReward? UseItemReward2 { get; set; }

        [JsonPropertyName("api_return_flag")]
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool CanRecall { get; set; }

        [JsonPropertyName("api_win_mat_level")]
        public Materials MaterialRewardLevel { get; set; }
        [JsonPropertyName("api_sample_fleet")]
        public IReadOnlyList<ShipTypeId> SampleFleet { get; set; }
    }
#nullable enable
}
