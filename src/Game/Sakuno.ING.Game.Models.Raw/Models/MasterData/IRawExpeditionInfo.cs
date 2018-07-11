using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
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
        public override bool Equals(object obj) => (ExpeditionId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public interface IRawExpeditionInfo : IIdentifiable<ExpeditionId>
    {
        string DisplayId { get; }
        MapAreaId MapAreaId { get; }

        string Name { get; }
        string Description { get; }
        TimeSpan Duration { get; }

        int RequiredShipCount { get; }
        int Difficulty { get; }
        double FuelConsumption { get; }
        double BulletConsumption { get; }

        ItemRecord? RewardItem1 { get; }
        ItemRecord? RewardItem2 { get; }

        bool CanRecall { get; }
    }
}
