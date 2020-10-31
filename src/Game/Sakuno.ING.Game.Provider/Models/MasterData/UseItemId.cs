using System;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct UseItemId : IEquatable<UseItemId>, IComparable<UseItemId>
    {
        private readonly int value;
        public UseItemId(int value) => this.value = value;

        public int CompareTo(UseItemId other) => value - other.value;
        public bool Equals(UseItemId other) => value == other.value;

        public static implicit operator int(UseItemId id) => id.value;
        public static explicit operator UseItemId(int value) => new UseItemId(value);

        public static bool operator ==(UseItemId left, UseItemId right) => left.value == right.value;
        public static bool operator !=(UseItemId left, UseItemId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is UseItemId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
