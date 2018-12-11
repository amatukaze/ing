using System;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct FurnitureId : IEquatable<FurnitureId>, IComparable<FurnitureId>
    {
        private readonly int value;
        public FurnitureId(int value) => this.value = value;

        public int CompareTo(FurnitureId other) => value - other.value;
        public bool Equals(FurnitureId other) => value == other.value;

        public static implicit operator int(FurnitureId id) => id.value;
        public static explicit operator FurnitureId(int value) => new FurnitureId(value);

        public static bool operator ==(FurnitureId left, FurnitureId right) => left.value == right.value;
        public static bool operator !=(FurnitureId left, FurnitureId right) => left.value != right.value;
        public override bool Equals(object obj) => (FurnitureId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public interface IRawFurnitureInfo : IIdentifiable<FurnitureId>
    {
        int Type { get; }
        int CategoryNo { get; }
        string Name { get; }
        string Description { get; }

        int Rarity { get; }
        int Price { get; }
        bool IsSale { get; }
    }
}
