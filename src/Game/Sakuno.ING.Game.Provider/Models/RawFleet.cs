using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier(typeof(int))]
    public readonly struct FleetId : IEquatable<FleetId>, IComparable<FleetId>
    {
        private readonly int value;
        public FleetId(int value) => this.value = value;

        public int CompareTo(FleetId other) => value - other.value;
        public bool Equals(FleetId other) => value == other.value;

        public static implicit operator int(FleetId id) => id.value;
        public static explicit operator FleetId(int value) => new FleetId(value);

        public static bool operator ==(FleetId left, FleetId right) => left.value == right.value;
        public static bool operator !=(FleetId left, FleetId right) => left.value != right.value;
        public override bool Equals(object obj) => (FleetId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawFleet : IIdentifiable<FleetId>
    {
        internal RawFleet() { }

        [JsonProperty("api_id")]
        public FleetId Id { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }

        internal long[] api_mission;
        public FleetExpeditionState ExpeditionState => (FleetExpeditionState)api_mission.At(0);
        public ExpeditionId? ExpeditionId
        {
            get
            {
                long id = api_mission.At(1);
                if (id > 0)
                    return (ExpeditionId)id;
                else
                    return null;
            }
        }

        public DateTimeOffset? ExpeditionCompletionTime
        {
            get
            {
                long value = api_mission.At(2);
                return value > 0 ?
                    DateTimeOffset.FromUnixTimeMilliseconds(value) :
                    (DateTimeOffset?)null;
            }
        }

        [JsonProperty("api_ship")]
        public IReadOnlyList<ShipId> ShipIds { get; internal set; }
    }
}
