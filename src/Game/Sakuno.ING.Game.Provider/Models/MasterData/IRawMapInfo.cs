using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct MapId : IEquatable<MapId>, IComparable<MapId>
    {
        private readonly int value;
        public MapId(int value) => this.value = value;

        public int CompareTo(MapId other) => value - other.value;
        public bool Equals(MapId other) => value == other.value;

        public static implicit operator int(MapId id) => id.value;
        public static explicit operator MapId(int value) => new MapId(value);

        public static bool operator ==(MapId left, MapId right) => left.value == right.value;
        public static bool operator !=(MapId left, MapId right) => left.value != right.value;
        public override bool Equals(object obj) => (MapId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public interface IRawMapInfo : IIdentifiable<MapId>
    {
        MapAreaId MapAreaId { get; }
        int CategoryNo { get; }

        string Name { get; }
        int StarDifficulty { get; }
        string OperationName { get; }
        string Description { get; }

        IReadOnlyCollection<UseItemId> ItemAcquirements { get; }
        int? RequiredDefeatCount { get; }

        bool CanUseNormalFleet { get; }
        bool CanUseStrikingForceFleet { get; }
        bool CanUseCombinedFleet(CombinedFleetType type);

        IRawMapBgmInfo BgmInfo { get; }
    }
}
