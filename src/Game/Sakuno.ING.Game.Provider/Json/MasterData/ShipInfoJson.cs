using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class ShipInfoJson : IRawShipInfo
    {
        [JsonProperty("api_id")]
        public ShipInfoId Id { get; set; }
        [JsonProperty("api_sortno")]
        public int SortNo { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
        public string api_yomi;
        public string Phonetic => IsAbyssal ? string.Empty : api_yomi;
        [JsonProperty("api_getmes"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Introduction { get; set; }

        public bool IsAbyssal => Id > 1500;
        public string AbyssalClass => IsAbyssal ? api_yomi : string.Empty;

        [JsonProperty("api_stype")]
        public ShipTypeId TypeId { get; set; }
        [JsonProperty("api_ctype")]
        public int ClassId { get; set; }

        [JsonProperty("api_afterlv")]
        public int UpgradeLevel { get; set; }
        [JsonProperty("api_aftershipid")]
        public ShipInfoId? UpgradeTo { get; set; }
        public int api_afterfuel;
        public int api_afterbull;
        public Materials UpgradeConsumption => new Materials
        {
            Bullet = api_afterbull,
            Steel = api_afterfuel
        };

        public IReadOnlyCollection<ItemRecord> UpgradeSpecialConsumption { get; set; } = Array.Empty<ItemRecord>();

        [JsonProperty("api_taik"), JsonConverter(typeof(ShipMordenizationConverter))]
        public ShipMordenizationStatus HP { get; set; }

        [JsonProperty("api_souk"), JsonConverter(typeof(ShipMordenizationConverter))]
        public ShipMordenizationStatus Armor { get; set; }

        [JsonProperty("api_houg"), JsonConverter(typeof(ShipMordenizationConverter))]
        public ShipMordenizationStatus Firepower { get; set; }

        [JsonProperty("api_raig"), JsonConverter(typeof(ShipMordenizationConverter))]
        public ShipMordenizationStatus Torpedo { get; set; }

        [JsonProperty("api_tyku"), JsonConverter(typeof(ShipMordenizationConverter))]
        public ShipMordenizationStatus AntiAir { get; set; }

        [JsonProperty("api_luck"), JsonConverter(typeof(ShipMordenizationConverter))]
        public ShipMordenizationStatus Luck { get; set; }

        [JsonProperty("api_soku")]
        public ShipSpeed Speed { get; set; }
        [JsonProperty("api_leng")]
        public FireRange FireRange { get; set; }

        [JsonProperty("api_slot_num")]
        public int SlotCount { get; set; }
        [JsonProperty("api_maxeq")]
        public IReadOnlyList<int> Aircraft { get; set; }
        [JsonProperty("api_backs")]
        public int Rarity { get; set; }

        public int api_buildtime;
        public TimeSpan ConstructionTime => TimeSpan.FromMinutes(api_buildtime);

        [JsonProperty("api_broken"), JsonConverter(typeof(MaterialsConverter))]
        public Materials DismantleAcquirement { get; set; }

        [JsonProperty("api_powup")]
        public IReadOnlyList<int> PowerupWorth { get; set; }

        [JsonProperty("api_fuel_max")]
        public int FuelConsumption { get; set; }
        [JsonProperty("api_bull_max")]
        public int BulletConsumption { get; set; }
    }
}
