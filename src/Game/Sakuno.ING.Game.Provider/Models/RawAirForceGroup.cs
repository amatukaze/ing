using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier(typeof(int))]
    public readonly struct AirForceGroupId : IEquatable<AirForceGroupId>, IComparable<AirForceGroupId>
    {
        private readonly int value;
        public AirForceGroupId(int value) => this.value = value;

        public int CompareTo(AirForceGroupId other) => value - other.value;
        public bool Equals(AirForceGroupId other) => value == other.value;

        public static implicit operator int(AirForceGroupId id) => id.value;
        public static explicit operator AirForceGroupId(int value) => new AirForceGroupId(value);

        public static bool operator ==(AirForceGroupId left, AirForceGroupId right) => left.value == right.value;
        public static bool operator !=(AirForceGroupId left, AirForceGroupId right) => left.value != right.value;
        public override bool Equals(object obj) => (AirForceGroupId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawAirForceGroup : IIdentifiable<(MapAreaId MapArea, AirForceGroupId GroupId)>
    {
        internal RawAirForceGroup() { }
        public (MapAreaId, AirForceGroupId) Id => (MapAreaId, GroupId);

        [JsonProperty("api_rid")]
        public AirForceGroupId GroupId { get; internal set; }
        [JsonProperty("api_area_id")]
        public MapAreaId MapAreaId { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }

        internal struct Distance
        {
            public int api_base;
            public int api_bonus;
        }
        internal Distance api_distance;
        public int DistanceBase => api_distance.api_base;
        public int DistanceBonus => api_distance.api_bonus;

        [JsonProperty("api_action_kind")]
        public AirForceAction Action { get; internal set; }

        internal RawAirForceSquadron[] api_plane_info;
        public IReadOnlyCollection<RawAirForceSquadron> Squadrons => api_plane_info;
    }

    public enum AirForceAction
    {
        Standby = 0,
        Sortie = 1,
        Defense = 2,
        Retreat = 3,
        Rest = 4
    }
}
