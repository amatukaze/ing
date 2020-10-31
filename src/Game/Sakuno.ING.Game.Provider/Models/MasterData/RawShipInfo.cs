using Sakuno.ING.Game.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawShipInfo
    {
        [JsonPropertyName("api_id")]
        public ShipInfoId Id { get; set; }
        [JsonPropertyName("api_name")]
        public string Name { get; set; }

        public string api_yomi { get; set; }
        public string Phonetic => IsAbyssal ? null : api_yomi;
        [JsonPropertyName("api_getmes")]
        public string Introduction { get; set; }

        public bool IsAbyssal => Id > 1500;
        public string AbyssalClass => IsAbyssal ? api_yomi : null;

        [JsonPropertyName("api_stype")]
        public ShipTypeId TypeId { get; set; }
        [JsonPropertyName("api_ctype")]
        public int ClassId { get; set; }

        [JsonPropertyName("api_afterlv")]
        public int RemodelLevel { get; set; }
        [JsonPropertyName("api_aftershipid")]
        public ShipInfoId? RemodelTo { get; set; }

        public int api_afterfuel { get; set; }
        public int api_afterbull { get; set; }
        public Materials RemodelConsumption => new Materials()
        {
            Bullet = api_afterbull,
            Steel = api_afterfuel
        };

        [JsonPropertyName("api_taik")]
        /// <summary>耐久</summary>
        public ShipModernizationStatus HP { get; set; }
        [JsonPropertyName("api_souk")]
        /// <summary>装甲</summary>
        public ShipModernizationStatus Armor { get; set; }
        [JsonPropertyName("api_houg")]
        /// <summary>火力</summary>
        public ShipModernizationStatus Firepower { get; set; }
        [JsonPropertyName("api_raig")]
        /// <summary>雷装</summary>
        public ShipModernizationStatus Torpedo { get; set; }
        [JsonPropertyName("api_tyku")]
        /// <summary>対空</summary>
        public ShipModernizationStatus AntiAir { get; set; }
        [JsonPropertyName("api_luck")]
        /// <summary>運</summary>
        public ShipModernizationStatus Luck { get; set; }
        [JsonPropertyName("api_soku")]
        /// <summary>速力</summary>
        public ShipSpeed Speed { get; set; }
        /// <summary>射程</summary>
        [JsonPropertyName("api_leng")]
        public FireRange FireRange { get; set; }

        [JsonPropertyName("api_slot_num")]
        public int SlotCount { get; set; }
        [JsonPropertyName("api_maxeq")]
        public IReadOnlyList<int> PlaneCapacities { get; set; }
        [JsonPropertyName("api_backs")]
        public int Rarity { get; set; }

        [JsonPropertyName("api_buildtime")]
        [JsonConverter(typeof(TimeSpanInMinuteConverter))]
        public TimeSpan ConstructionTime { get; set; }

        [JsonPropertyName("api_broken")]
        public Materials DismantleAcquirement { get; set; }

        [JsonPropertyName("api_powup")]
        public IReadOnlyList<int> PowerUpAmount { get; set; }

        [JsonPropertyName("api_fuel_max")]
        public int FuelConsumption { get; set; }
        [JsonPropertyName("api_bull_max")]
        public int BulletConsumption { get; set; }
    }
#nullable enable
}
