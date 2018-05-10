using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    public readonly struct ExpeditionId : IEquatable<ExpeditionId>, IComparable<ExpeditionId>
    {
        private readonly int value;
        public ExpeditionId(int value) => this.value = value;

        public int CompareTo(ExpeditionId other) => value - other.value;
        public bool Equals(ExpeditionId other) => value == other.value;

        public static implicit operator int(ExpeditionId id) => id.value;
        public static explicit operator ExpeditionId(long value) => new ExpeditionId((int)value);

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

        IReadOnlyList<ItemRecord> RewardItems { get; }

        bool CanRecall { get; }
    }
}
