using System;
using Newtonsoft.Json;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct MapAreaId : IEquatable<MapAreaId>, IComparable<MapAreaId>
    {
        private readonly int value;
        public MapAreaId(int value) => this.value = value;

        public int CompareTo(MapAreaId other) => value - other.value;
        public bool Equals(MapAreaId other) => value == other.value;

        public static implicit operator int(MapAreaId id) => id.value;
        public static explicit operator MapAreaId(int value) => new MapAreaId(value);

        public static bool operator ==(MapAreaId left, MapAreaId right) => left.value == right.value;
        public static bool operator !=(MapAreaId left, MapAreaId right) => left.value != right.value;
        public override bool Equals(object obj) => (MapAreaId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawMapArea : IIdentifiable<MapAreaId>
    {
        internal RawMapArea() { }

        [JsonProperty("api_id")]
        public MapAreaId Id { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }
        [JsonProperty("api_type")]
        public bool IsEvent { get; internal set; }
    }
}
