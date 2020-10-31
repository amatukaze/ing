using System;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct SlotItemInfoId : IEquatable<SlotItemInfoId>, IComparable<SlotItemInfoId>
    {
        private readonly int value;
        public SlotItemInfoId(int value) => this.value = value;

        public int CompareTo(SlotItemInfoId other) => value - other.value;
        public bool Equals(SlotItemInfoId other) => value == other.value;

        public static implicit operator int(SlotItemInfoId id) => id.value;
        public static explicit operator SlotItemInfoId(int value) => new SlotItemInfoId(value);

        public static bool operator ==(SlotItemInfoId left, SlotItemInfoId right) => left.value == right.value;
        public static bool operator !=(SlotItemInfoId left, SlotItemInfoId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is SlotItemInfoId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
