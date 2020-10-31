using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct SlotItemTypeId : IEquatable<SlotItemTypeId>, IComparable<SlotItemTypeId>
    {
        private readonly int value;
        public SlotItemTypeId(int value) => this.value = value;

        public int CompareTo(SlotItemTypeId other) => value - other.value;
        public bool Equals(SlotItemTypeId other) => value == other.value;

        public static implicit operator int(SlotItemTypeId id) => id.value;
        public static explicit operator SlotItemTypeId(int value) => new SlotItemTypeId(value);

        public static bool operator ==(SlotItemTypeId left, SlotItemTypeId right) => left.value == right.value;
        public static bool operator !=(SlotItemTypeId left, SlotItemTypeId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is SlotItemTypeId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
