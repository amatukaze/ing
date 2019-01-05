using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier(typeof(int))]
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
        public override bool Equals(object obj) => (QuestId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public interface IRawQuest : IIdentifiable<QuestId>
    {
        QuestCategoty Category { get; }
        QuestPeriod Period { get; }
        QuestState State { get; }
        QuestProgress Progress { get; }
        string Name { get; }
        string Description { get; }
        Materials Rewards { get; }
    }

    public enum QuestCategoty
    {
        Composition = 1,
        Sortie = 2,
        Exercise = 3,
        Expedition = 4,
        Supply = 5,
        Arsenal = 6,
        Mordenization = 7
    }

    public enum QuestPeriod
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Once = 4,
        Quarterly = 5
    }

    public enum QuestState
    {
        Inactive = 1,
        Active = 2,
        Completed = 3
    }

    public enum QuestProgress
    {
        None = 0,
        Half = 1,
        Almost = 2
    }
}
