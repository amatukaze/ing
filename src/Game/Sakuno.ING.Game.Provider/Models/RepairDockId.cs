using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct RepairDockId : IEquatable<RepairDockId>, IComparable<RepairDockId>
    {
        private readonly int value;
        public RepairDockId(int value) => this.value = value;

        public int CompareTo(RepairDockId other) => value - other.value;
        public bool Equals(RepairDockId other) => value == other.value;

        public static implicit operator int(RepairDockId id) => id.value;
        public static explicit operator RepairDockId(int value) => new RepairDockId(value);

        public static bool operator ==(RepairDockId left, RepairDockId right) => left.value == right.value;
        public static bool operator !=(RepairDockId left, RepairDockId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is RepairDockId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
