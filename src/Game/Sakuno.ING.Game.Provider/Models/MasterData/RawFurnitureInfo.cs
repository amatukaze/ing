using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct FurnitureId : IEquatable<FurnitureId>, IComparable<FurnitureId>
    {
        private readonly int value;
        public FurnitureId(int value) => this.value = value;

        public int CompareTo(FurnitureId other) => value - other.value;
        public bool Equals(FurnitureId other) => value == other.value;

        public static implicit operator int(FurnitureId id) => id.value;
        public static explicit operator FurnitureId(int value) => new FurnitureId(value);

        public static bool operator ==(FurnitureId left, FurnitureId right) => left.value == right.value;
        public static bool operator !=(FurnitureId left, FurnitureId right) => left.value != right.value;
        public override bool Equals(object obj) => (FurnitureId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawFurnitureInfo : IIdentifiable<FurnitureId>
    {
        internal RawFurnitureInfo() { }

        [JsonProperty("api_id")]
        public FurnitureId Id { get; internal set; }
        [JsonProperty("api_type")]
        public int Type { get; internal set; }
        [JsonProperty("api_no")]
        public int CategoryNo { get; internal set; }
        [JsonProperty("api_title")]
        public string Name { get; internal set; }
        [JsonProperty("api_description"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; internal set; }

        [JsonProperty("api_rarity")]
        public int Rarity { get; internal set; }
        [JsonProperty("api_price")]
        public int Price { get; internal set; }
        [JsonProperty("api_saleflg")]
        public bool IsSale { get; internal set; }
    }
}
