using Sakuno.ING.Game.Json.Converters;
using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct QuestId : IEquatable<QuestId>, IComparable<QuestId>
    {
        private readonly int value;
        public QuestId(int value) => this.value = value;

        public int CompareTo(QuestId other) => value - other.value;
        public bool Equals(QuestId other) => value == other.value;

        public static implicit operator int(QuestId id) => id.value;
        public static explicit operator QuestId(int value) => new QuestId(value);

        public static bool operator ==(QuestId left, QuestId right) => left.value == right.value;
        public static bool operator !=(QuestId left, QuestId right) => left.value != right.value;
        public override bool Equals(object obj) => obj is QuestId other && other == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }
}
