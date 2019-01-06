using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct EquipmentTypeId : IEquatable<EquipmentTypeId>, IComparable<EquipmentTypeId>
    {
        private readonly int value;
        public EquipmentTypeId(int value) => this.value = value;

        public int CompareTo(EquipmentTypeId other) => value - other.value;
        public bool Equals(EquipmentTypeId other) => value == other.value;

        public static implicit operator int(EquipmentTypeId id) => id.value;
        public static explicit operator EquipmentTypeId(int value) => new EquipmentTypeId(value);

        public static implicit operator EquipmentTypeId(KnownEquipmentType known) => new EquipmentTypeId((int)known);
        public static explicit operator KnownEquipmentType(EquipmentTypeId id) => (KnownEquipmentType)id.value;

        public static bool operator ==(EquipmentTypeId left, EquipmentTypeId right) => left.value == right.value;
        public static bool operator !=(EquipmentTypeId left, EquipmentTypeId right) => left.value != right.value;
        public override bool Equals(object obj) => (EquipmentTypeId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawEquipmentTypeInfo : IIdentifiable<EquipmentTypeId>
    {
        internal RawEquipmentTypeInfo() { }

        [JsonProperty("api_id")]
        public EquipmentTypeId Id { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }

        public bool AvailableInExtraSlot { get; internal set; }
    }
}
