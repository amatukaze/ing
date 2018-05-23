using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct MapAreaId : IEquatable<MapAreaId>, IComparable<MapAreaId>
    {
        private readonly int value;
        public MapAreaId(int value) => this.value = value;

        public int CompareTo(MapAreaId other) => value - other.value;
        public bool Equals(MapAreaId other) => value == other.value;

        public static implicit operator int(MapAreaId id) => id.value;
        public static explicit operator MapAreaId(int value) => new MapAreaId(value);

        public override string ToString() => value.ToString();
    }

    public interface IRawMapArea : IIdentifiable<MapAreaId>
    {
        string Name { get; }
        bool IsEvent { get; }
    }
}
