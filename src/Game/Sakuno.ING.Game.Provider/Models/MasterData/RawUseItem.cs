using System;
using Newtonsoft.Json;
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

        public static bool operator ==(UseItemId left, UseItemId right) => left.value == right.value;
        public static bool operator !=(UseItemId left, UseItemId right) => left.value != right.value;
        public override bool Equals(object obj) => (UseItemId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawUseItem : IIdentifiable<UseItemId>
    {
        internal RawUseItem() { }

        [JsonProperty("api_id")]
        public UseItemId Id { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }
    }
}
