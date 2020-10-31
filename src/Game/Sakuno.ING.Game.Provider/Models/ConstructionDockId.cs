using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct ConstructionDockId : IEquatable<ConstructionDockId>, IComparable<ConstructionDockId>
    {
        private readonly int value;
        public ConstructionDockId(int value) => this.value = value;

        public int CompareTo(ConstructionDockId other) => value - other.value;
        public bool Equals(ConstructionDockId other) => value == other.value;

        public static implicit operator int(ConstructionDockId id) => id.value;
        public static explicit operator ConstructionDockId(int value) => new ConstructionDockId(value);

        public static bool operator ==(ConstructionDockId left, ConstructionDockId right) => left.value == right.value;
        public static bool operator !=(ConstructionDockId left, ConstructionDockId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is ConstructionDockId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
