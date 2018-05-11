using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public readonly struct FleetId : IEquatable<FleetId>, IComparable<FleetId>
    {
        private readonly int value;
        public FleetId(int value) => this.value = value;

        public int CompareTo(FleetId other) => value - other.value;
        public bool Equals(FleetId other) => value == other.value;

        public static implicit operator int(FleetId id) => id.value;
        public static explicit operator FleetId(long value) => new FleetId((int)value);

        public override string ToString() => value.ToString();
    }

    public interface IRawFleet : IIdentifiable<FleetId>
    {
        string Name { get; }
        FleetExpeditionState ExpeditionState { get; }
        ExpeditionId ExpeditionId { get; }
        DateTimeOffset ExpeditionCompletionTime { get; }
        IReadOnlyList<ShipId> ShipIds { get; }
    }
}
