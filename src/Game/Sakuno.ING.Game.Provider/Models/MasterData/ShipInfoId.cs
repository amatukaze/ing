using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct ShipInfoId : IEquatable<ShipInfoId>, IComparable<ShipInfoId>
    {
        private readonly int value;
        public ShipInfoId(int value) => this.value = value;

        public int CompareTo(ShipInfoId other) => value - other.value;
        public bool Equals(ShipInfoId other) => value == other.value;

        public static implicit operator int(ShipInfoId id) => id.value;
        public static explicit operator ShipInfoId(int value) => new ShipInfoId(value);

        public static bool operator ==(ShipInfoId left, ShipInfoId right) => left.value == right.value;
        public static bool operator !=(ShipInfoId left, ShipInfoId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is ShipInfoId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
