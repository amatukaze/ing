using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct FleetId : IEquatable<FleetId>, IComparable<FleetId>
    {
        private readonly int value;
        public FleetId(int value) => this.value = value;

        public int CompareTo(FleetId other) => value - other.value;
        public bool Equals(FleetId other) => value == other.value;

        public static implicit operator int(FleetId id) => id.value;
        public static explicit operator FleetId(int value) => new FleetId(value);

        public static bool operator ==(FleetId left, FleetId right) => left.value == right.value;
        public static bool operator !=(FleetId left, FleetId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is FleetId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
