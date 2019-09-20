using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct BuildingDockId : IEquatable<BuildingDockId>, IComparable<BuildingDockId>
    {
        private readonly int value;
        public BuildingDockId(int value) => this.value = value;

        public int CompareTo(BuildingDockId other) => value - other.value;
        public bool Equals(BuildingDockId other) => value == other.value;

        public static implicit operator int(BuildingDockId id) => id.value;
        public static explicit operator BuildingDockId(int value) => new BuildingDockId(value);

        public static bool operator ==(BuildingDockId left, BuildingDockId right) => left.value == right.value;
        public static bool operator !=(BuildingDockId left, BuildingDockId right) => left.value != right.value;
        public override bool Equals(object obj) => (BuildingDockId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawBuildingDock : IIdentifiable<BuildingDockId>
    {
        internal RawBuildingDock() { }

        [JsonProperty("api_id")]
        public BuildingDockId Id { get; internal set; }

        [JsonProperty("api_complete_time")]
        public DateTimeOffset? CompletionTime { get; internal set; }

        [JsonProperty("api_state")]
        public BuildingDockState State { get; internal set; }

        internal int api_item1, api_item2, api_item3, api_item4, api_item5;
        public Materials Consumption => new Materials
        {
            Fuel = api_item1,
            Bullet = api_item2,
            Steel = api_item3,
            Bauxite = api_item4,
            Development = api_item5
        };

        [JsonProperty("api_created_ship_id")]
        public ShipInfoId? BuiltShipId { get; internal set; }

        public bool IsLSC => api_item1 >= 1000;
    }

    public enum BuildingDockState
    {
        Locked = -1,
        Empty = 0,
        Building = 2,
        BuildCompleted = 3,
    }
}
