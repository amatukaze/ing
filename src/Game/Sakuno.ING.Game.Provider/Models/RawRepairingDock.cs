using System;
using Newtonsoft.Json;

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

    public sealed class RawRepairingDock : IIdentifiable<RepairingDockId>
    {
        internal RawRepairingDock() { }

        [JsonProperty("api_id")]
        public RepairingDockId Id { get; internal set; }
        [JsonProperty("api_state")]
        public RepairingDockState State { get; internal set; }
        [JsonProperty("api_ship_id")]
        public ShipId? RepairingShipId { get; internal set; }
        [JsonProperty("api_complete_time")]
        public DateTimeOffset? CompletionTime { get; internal set; }

        internal int api_item1;
        internal int api_item3;
        public Materials Consumption => new Materials
        {
            Fuel = api_item1,
            Steel = api_item3
        };
    }

    public enum RepairingDockState
    {
        Locked = -1,
        Empty = 0,
        Repairing = 1,
    }
}
