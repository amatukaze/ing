using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct ShipTypeId : IEquatable<ShipTypeId>, IComparable<ShipTypeId>
    {
        private readonly int value;
        public ShipTypeId(int value) => this.value = value;

        public int CompareTo(ShipTypeId other) => value - other.value;
        public bool Equals(ShipTypeId other) => value == other.value;

        public static implicit operator int(ShipTypeId id) => id.value;
        public static explicit operator ShipTypeId(int value) => new ShipTypeId(value);

        public static bool operator ==(ShipTypeId left, ShipTypeId right) => left.value == right.value;
        public static bool operator !=(ShipTypeId left, ShipTypeId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is ShipInfoId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
#nullable enable
}
