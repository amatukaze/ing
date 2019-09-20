using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier]
    public readonly struct ShipInfoId : IEquatable<ShipInfoId>, IComparable<ShipInfoId>
    {
        private readonly int value;
        public ShipInfoId(int value) => this.value = value;

        public int CompareTo(ShipInfoId other) => value - other.value;
        public bool Equals(ShipInfoId other) => value == other.value;

        public static implicit operator int(ShipInfoId id) => id.value;
        public static explicit operator ShipInfoId(int value) => new ShipInfoId(value);

        public static bool operator ==(ShipInfoId left, ShipInfoId right) => left.value == right.value;
        public static bool operator !=(ShipInfoId left, ShipInfoId right) => left.value != right.value;
        public override bool Equals(object obj) => (ShipInfoId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
    }

    public sealed class RawShipInfo : IIdentifiable<ShipInfoId>
    {
        internal RawShipInfo() { }

        [JsonProperty("api_id")]
        public ShipInfoId Id { get; internal set; }
        [JsonProperty("api_sortno")]
        public int SortNo { get; internal set; }
        [JsonProperty("api_name")]
        public string Name { get; internal set; }

        internal string api_yomi;
        public string Phonetic => IsAbyssal ? null : api_yomi;
        [JsonProperty("api_getmes"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Introduction { get; internal set; }

        public bool IsAbyssal => Id > 1500;
        public string AbyssalClass => IsAbyssal ? api_yomi : null;

        [JsonProperty("api_stype")]
        public ShipTypeId TypeId { get; internal set; }
        [JsonProperty("api_ctype")]
        public int ClassId { get; internal set; }

        [JsonProperty("api_afterlv")]
        public int UpgradeLevel { get; internal set; }
        [JsonProperty("api_aftershipid")]
        public ShipInfoId? UpgradeTo { get; internal set; }

        internal int api_afterfuel;
        internal int api_afterbull;
        public Materials UpgradeConsumption => new Materials
        {
            Bullet = api_afterbull,
            Steel = api_afterfuel
        };

        public IReadOnlyCollection<UseItemRecord> UpgradeSpecialConsumption { get; internal set; } = Array.Empty<UseItemRecord>();

        [JsonProperty("api_taik"), JsonConverter(typeof(ShipMordenizationConverter))]
        /// <summary>耐久</summary>
        public ShipMordenizationStatus HP { get; internal set; }

        [JsonProperty("api_souk"), JsonConverter(typeof(ShipMordenizationConverter))]
        /// <summary>装甲</summary>
        public ShipMordenizationStatus Armor { get; internal set; }

        [JsonProperty("api_houg"), JsonConverter(typeof(ShipMordenizationConverter))]
        /// <summary>火力</summary>
        public ShipMordenizationStatus Firepower { get; internal set; }

        [JsonProperty("api_raig"), JsonConverter(typeof(ShipMordenizationConverter))]
        /// <summary>雷装</summary>
        public ShipMordenizationStatus Torpedo { get; internal set; }

        [JsonProperty("api_tyku"), JsonConverter(typeof(ShipMordenizationConverter))]
        /// <summary>対空</summary>
        public ShipMordenizationStatus AntiAir { get; internal set; }

        [JsonProperty("api_luck"), JsonConverter(typeof(ShipMordenizationConverter))]
        /// <summary>運</summary>
        public ShipMordenizationStatus Luck { get; internal set; }

        [JsonProperty("api_soku")]
        /// <summary>速力</summary>
        public ShipSpeed Speed { get; internal set; }
        /// <summary>射程</summary>
        [JsonProperty("api_leng")]
        public FireRange FireRange { get; internal set; }

        [JsonProperty("api_slot_num")]
        public int SlotCount { get; internal set; }
        [JsonProperty("api_maxeq")]
        public IReadOnlyList<int> Aircraft { get; internal set; }
        [JsonProperty("api_backs")]
        public int Rarity { get; internal set; }

        internal int api_buildtime;
        public TimeSpan ConstructionTime => TimeSpan.FromMinutes(api_buildtime);

        [JsonProperty("api_broken"), JsonConverter(typeof(MaterialsConverter))]
        public Materials DismantleAcquirement { get; internal set; }

        [JsonProperty("api_powup")]
        public IReadOnlyList<int> PowerupWorth { get; internal set; }

        [JsonProperty("api_fuel_max")]
        public int FuelConsumption { get; internal set; }
        [JsonProperty("api_bull_max")]
        public int BulletConsumption { get; internal set; }

        public IReadOnlyCollection<EquipmentTypeId> OverrideAvailableEquipmentTypes { get; internal set; }
    }
}
