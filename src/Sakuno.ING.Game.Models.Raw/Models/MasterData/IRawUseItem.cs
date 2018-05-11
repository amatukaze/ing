using System;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct UseItemId : IEquatable<UseItemId>, IComparable<UseItemId>
    {
        private readonly int value;
        public UseItemId(int value) => this.value = value;

        public int CompareTo(UseItemId other) => value - other.value;
        public bool Equals(UseItemId other) => value == other.value;

        public static implicit operator int(UseItemId id) => id.value;
        public static explicit operator UseItemId(int value) => new UseItemId(value);

        public static implicit operator UseItemId(KnownUseItem known) => new UseItemId((int)known);
        public static explicit operator KnownUseItem(UseItemId id) => (KnownUseItem)id.value;

        public override string ToString() => value.ToString();
    }

    public interface IRawUseItem : IIdentifiable<UseItemId>
    {
        string Name { get; }
    }
}
