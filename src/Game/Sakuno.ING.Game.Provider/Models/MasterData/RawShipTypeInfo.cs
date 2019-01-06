using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct ShipTypeId : IEquatable<ShipTypeId>, IComparable<ShipTypeId>
    {
        private readonly int value;
        public ShipTypeId(int value) => this.value = value;

        public int CompareTo(ShipTypeId other) => value - other.value;
        public bool Equals(ShipTypeId other) => value == other.value;

        public static implicit operator int(ShipTypeId id) => id.value;
        public static explicit operator ShipTypeId(int value) => new ShipTypeId(value);

        public static implicit operator ShipTypeId(KnownShipType known) => new ShipTypeId((int)known);
        public static explicit operator KnownShipType(ShipTypeId id) => (KnownShipType)id.value;

        public static bool operator ==(ShipTypeId left, ShipTypeId right) => left.value == right.value;
        public static bool operator !=(ShipTypeId left, ShipTypeId right) => left.value != right.value;
        public override bool Equals(object obj) => (ShipTypeId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawShipTypeInfo : IIdentifiable<ShipTypeId>
    {
        internal RawShipTypeInfo() { }

        [JsonProperty("api_id")]
        public ShipTypeId Id { get; internal set; }
        [JsonProperty("api_sortno")]
        public int SortNo { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }

        [JsonProperty("api_scnt")]
        public int RepairTimeRatio { get; internal set; }
        [JsonProperty("api_kcnt")]
        public int BuildOutlineId { get; internal set; }

        [JsonProperty("api_equip_type"), JsonConverter(typeof(BoolDictionaryConverter))]
        public IReadOnlyCollection<EquipmentTypeId> AvailableEquipmentTypes { get; internal set; }
    }
}
