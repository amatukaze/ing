using System;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct ExpeditionId : IEquatable<ExpeditionId>, IComparable<ExpeditionId>
    {
        private readonly int value;
        public ExpeditionId(int value) => this.value = value;

        public int CompareTo(ExpeditionId other) => value - other.value;
        public bool Equals(ExpeditionId other) => value == other.value;

        public static implicit operator int(ExpeditionId id) => id.value;
        public static explicit operator ExpeditionId(int value) => new ExpeditionId(value);

        public static bool operator ==(ExpeditionId left, ExpeditionId right) => left.value == right.value;
        public static bool operator !=(ExpeditionId left, ExpeditionId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is ExpeditionId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
