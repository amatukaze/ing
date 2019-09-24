using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct ExpeditionId : IEquatable<ExpeditionId>, IComparable<ExpeditionId>
    {
        private readonly int value;
        public ExpeditionId(int value) => this.value = value;

        public int CompareTo(ExpeditionId other) => value - other.value;
        public bool Equals(ExpeditionId other) => value == other.value;

        public static implicit operator int(ExpeditionId id) => id.value;
        public static explicit operator ExpeditionId(int value) => new ExpeditionId(value);

        public static bool operator ==(ExpeditionId left, ExpeditionId right) => left.value == right.value;
        public static bool operator !=(ExpeditionId left, ExpeditionId right) => left.value != right.value;
        public override bool Equals(object obj) => (ExpeditionId)obj == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }

    public sealed class RawExpeditionInfo : IIdentifiable<ExpeditionId>
    {
        internal RawExpeditionInfo() { }

        [JsonProperty("api_id")]
        public ExpeditionId Id { get; internal set; }
        [JsonProperty("api_disp_no")]
        public string DisplayId { get; internal set; }
        [JsonProperty("api_maparea_id")]
        public MapAreaId MapAreaId { get; internal set; }

        [JsonProperty("api_name")]
        public string Name { get; internal set; }
        [JsonProperty("api_details"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; internal set; }
        public int api_time;
        public TimeSpan Duration => TimeSpan.FromMinutes(api_time);

        [JsonProperty("api_deck_num")]
        public int RequiredShipCount { get; internal set; }
        [JsonProperty("api_difficulty")]
        public int Difficulty { get; internal set; }
        [JsonProperty("api_use_fuel")]
        public double FuelConsumption { get; internal set; }
        [JsonProperty("api_use_bull")]
        public double BulletConsumption { get; internal set; }

        [JsonProperty("api_win_item1"), JsonConverter(typeof(ItemRecordConverter))]
        public UseItemRecord? RewardItem1 { get; internal set; }

        [JsonProperty("api_win_item2"), JsonConverter(typeof(ItemRecordConverter))]
        public UseItemRecord? RewardItem2 { get; internal set; }

        [JsonProperty("api_return_flag")]
        public bool CanRecall { get; internal set; }

        [JsonProperty("api_win_mat_level"), JsonConverter(typeof(MaterialsConverter))]
        public Materials MaterialRewardsLevel { get; internal set; }
        [JsonProperty("api_sample_fleet")]
        public IReadOnlyList<ShipTypeId> SampleFleet { get; internal set; }
    }
}
