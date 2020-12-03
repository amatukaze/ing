using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct AirForceGroupId : IEquatable<AirForceGroupId>, IComparable<AirForceGroupId>
    {
        private readonly int value;
        public AirForceGroupId(int value) => this.value = value;

        public int CompareTo(AirForceGroupId other) => value - other.value;
        public bool Equals(AirForceGroupId other) => value == other.value;

        public static implicit operator int(AirForceGroupId id) => id.value;
        public static explicit operator AirForceGroupId(int value) => new AirForceGroupId(value);

        public static bool operator ==(AirForceGroupId left, AirForceGroupId right) => left.value == right.value;
        public static bool operator !=(AirForceGroupId left, AirForceGroupId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is AirForceGroupId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
