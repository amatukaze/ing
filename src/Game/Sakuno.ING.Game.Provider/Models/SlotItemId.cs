using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct SlotItemId : IEquatable<SlotItemId>, IComparable<SlotItemId>
    {
        private readonly int value;
        public SlotItemId(int value) => this.value = value;

        public int CompareTo(SlotItemId other) => value - other.value;
        public bool Equals(SlotItemId other) => value == other.value;

        public static implicit operator int(SlotItemId id) => id.value;
        public static explicit operator SlotItemId(int value) => new SlotItemId(value);

        public static bool operator ==(SlotItemId left, SlotItemId right) => left.value == right.value;
        public static bool operator !=(SlotItemId left, SlotItemId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is SlotItemId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
