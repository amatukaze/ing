using System;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct MapId : IEquatable<MapId>, IComparable<MapId>
    {
        private readonly int value;
        public MapId(int value) => this.value = value;

        public MapAreaId AreaId => (MapAreaId)(value / 10);
        public int CategoryNo => value % 10;

        public int CompareTo(MapId other) => value - other.value;
        public bool Equals(MapId other) => value == other.value;

        public static implicit operator int(MapId id) => id.value;
        public static explicit operator MapId(int value) => new MapId(value);

        public static bool operator ==(MapId left, MapId right) => left.value == right.value;
        public static bool operator !=(MapId left, MapId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is MapId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString()
        {
            var area = Math.DivRem(value, 10, out var category);

            return $"{area}-{category}";
        }
    }
}
