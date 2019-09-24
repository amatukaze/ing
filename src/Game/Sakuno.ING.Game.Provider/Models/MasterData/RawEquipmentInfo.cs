using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct EquipmentInfoId : IEquatable<EquipmentInfoId>, IComparable<EquipmentInfoId>
    {
        private readonly int value;
        public EquipmentInfoId(int value) => this.value = value;

        public int CompareTo(EquipmentInfoId other) => value - other.value;
        public bool Equals(EquipmentInfoId other) => value == other.value;

        public static implicit operator int(EquipmentInfoId id) => id.value;
        public static explicit operator EquipmentInfoId(int value) => new EquipmentInfoId(value);

        public static bool operator ==(EquipmentInfoId left, EquipmentInfoId right) => left.value == right.value;
        public static bool operator !=(EquipmentInfoId left, EquipmentInfoId right) => left.value != right.value;
        public override bool Equals(object obj) => (EquipmentInfoId)obj == this;
        public override int GetHashCode() => value;
        public override string ToString() => value.ToString();
    }

    public sealed class RawEquipmentInfo : IIdentifiable<EquipmentInfoId>
    {
        internal RawEquipmentInfo() { }

        [JsonProperty("api_id")]
        public EquipmentInfoId Id { get; internal set; }

        [JsonProperty("api_name")]
        public string Name { get; internal set; }

        [JsonProperty("api_info"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; internal set; }

        internal int[] api_type;
        public EquipmentTypeId TypeId => (EquipmentTypeId)api_type.At(2);
        public int IconId => api_type.At(3);

        public IReadOnlyCollection<ShipInfoId> ExtraSlotAcceptingShips { get; internal set; } = Array.Empty<ShipInfoId>();

        [JsonProperty("api_houg")]
        /// <summary>火力</summary>
        public int Firepower { get; internal set; }

        [JsonProperty("api_raig")]
        /// <summary>雷装</summary>
        public int Torpedo { get; internal set; }

        [JsonProperty("api_tyku")]
        /// <summary>対空</summary>
        public int AntiAir { get; internal set; }

        [JsonProperty("api_souk")]
        /// <summary>装甲</summary>
        public int Armor { get; internal set; }

        [JsonProperty("api_baku")]
        /// <summary>爆装</summary>
        public int DiveBomberAttack { get; internal set; }

        [JsonProperty("api_tais")]
        /// <summary>対潜</summary>
        public int AntiSubmarine { get; internal set; }

        internal int api_houm;
        internal int api_houk;
        /// <summary>命中</summary>
        public int Accuracy => TypeId != 48 ? api_houm : 0;
        /// <summary>回避</summary>
        public int Evasion => TypeId != 48 ? api_houk : 0;
        /// <summary>対爆</summary>
        public int AntiBomber => TypeId == 48 ? api_houm : 0;
        /// <summary>迎撃</summary>
        public int Interception => TypeId == 48 ? api_houk : 0;

        [JsonProperty("api_saku")]
        /// <summary>索敵</summary>
        public int LineOfSight { get; internal set; }

        [JsonProperty("api_length")]
        public FireRange FireRange { get; internal set; }

        [JsonProperty("api_distance")]
        public int FlightRadius { get; internal set; }

        internal int api_cost;
        public Materials DeploymentConsumption => new Materials { Bauxite = api_cost };

        [JsonProperty("api_broken"), JsonConverter(typeof(MaterialsConverter))]
        public Materials DismantleAcquirement { get; internal set; }

        [JsonProperty("api_rare")]
        public int Rarity { get; internal set; }
    }
}
