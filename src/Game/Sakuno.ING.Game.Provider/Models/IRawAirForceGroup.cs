using System;
using System.Collections.Generic;
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

    public interface IRawAirForceGroup : IIdentifiable<(MapAreaId MapArea, AirForceGroupId GroupId)>
    {
        MapAreaId MapAreaId { get; }
        AirForceGroupId GroupId { get; }
        string Name { get; }
        int DistanceBase { get; }
        int DistanceBonus { get; }
        AirForceAction Action { get; }
        IReadOnlyCollection<IRawAirForceSquadron> Squadrons { get; }
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
