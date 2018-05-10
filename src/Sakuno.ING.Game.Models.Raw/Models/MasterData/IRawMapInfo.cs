using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    public readonly struct MapId : IEquatable<MapId>, IComparable<MapId>
    {
        private readonly int value;
        public MapId(int value) => this.value = value;

        public int CompareTo(MapId other) => value - other.value;
        public bool Equals(MapId other) => value == other.value;

        public static implicit operator int(MapId id) => id.value;
        public static explicit operator MapId(long value) => new MapId((int)value);

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

        IReadOnlyCollection<FleetType> AvailableFleetTypes { get; }

        IRawMapBgmInfo BgmInfo { get; }
    }
}
