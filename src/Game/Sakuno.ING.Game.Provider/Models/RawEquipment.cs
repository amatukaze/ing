using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier]
    public readonly struct EquipmentId : IEquatable<EquipmentId>, IComparable<EquipmentId>
    {
        private readonly int value;
        public EquipmentId(int value) => this.value = value;

        public int CompareTo(EquipmentId other) => value - other.value;
        public bool Equals(EquipmentId other) => value == other.value;

        public static implicit operator int(EquipmentId id) => id.value;
        public static explicit operator EquipmentId(int value) => new EquipmentId(value);

        public static bool operator ==(EquipmentId left, EquipmentId right) => left.value == right.value;
        public static bool operator !=(EquipmentId left, EquipmentId right) => left.value != right.value;
        public override bool Equals(object obj) => (EquipmentId)obj == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }

    public sealed class RawEquipment : IIdentifiable<EquipmentId>
    {
        internal RawEquipment() { }

        [JsonProperty("api_id")]
        public EquipmentId Id { get; internal set; }
        [JsonProperty("api_slotitem_id")]
        public EquipmentInfoId EquipmentInfoId { get; internal set; }
        [JsonProperty("api_locked")]
        public bool IsLocked { get; internal set; }
        [JsonProperty("api_level")]
        public int ImprovementLevel { get; internal set; }
        [JsonProperty("api_alv")]
        public int AirProficiency { get; internal set; }
    }
}
