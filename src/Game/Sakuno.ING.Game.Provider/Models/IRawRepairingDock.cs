using System;

namespace Sakuno.ING.Game.Models
{
    [Identifier(typeof(int))]
    public readonly struct RepairingDockId : IEquatable<RepairingDockId>, IComparable<RepairingDockId>
    {
        private readonly int value;
        public RepairingDockId(int value) => this.value = value;

        public int CompareTo(RepairingDockId other) => value - other.value;
        public bool Equals(RepairingDockId other) => value == other.value;

        public static implicit operator int(RepairingDockId id) => id.value;
        public static explicit operator RepairingDockId(int value) => new RepairingDockId(value);

        public static bool operator ==(RepairingDockId left, RepairingDockId right) => left.value == right.value;
        public static bool operator !=(RepairingDockId left, RepairingDockId right) => left.value != right.value;
        public override bool Equals(object obj) => (RepairingDockId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public interface IRawRepairingDock : IIdentifiable<RepairingDockId>
    {
        RepairingDockState State { get; }
        ShipId? RepairingShipId { get; }
        DateTimeOffset? CompletionTime { get; }
        Materials Consumption { get; }
    }

    public enum RepairingDockState
    {
        Locked = -1,
        Empty = 0,
        Repairing = 1,
    }
}
