using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct ShipId : IEquatable<ShipId>, IComparable<ShipId>
    {
        private readonly int value;
        public ShipId(int value) => this.value = value;

        public int CompareTo(ShipId other) => value - other.value;
        public bool Equals(ShipId other) => value == other.value;

        public static implicit operator int(ShipId id) => id.value;
        public static explicit operator ShipId(int value) => new ShipId(value);

        public static bool operator ==(ShipId left, ShipId right) => left.value == right.value;
        public static bool operator !=(ShipId left, ShipId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is ShipId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
