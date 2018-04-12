using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class ShipInfoJson : IRawShipInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_sortno")]
        public int SortNo { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
        public string api_yomi;
        public string Phonetic => IsAbyssal ? string.Empty : api_yomi;
        [JsonProperty("api_getmes", ItemConverterType = typeof(HtmlNewLineEater))]
        public string Introduction { get; set; }

        public bool IsAbyssal => Id > 1500;
        public string AbyssalClass => IsAbyssal ? api_yomi : string.Empty;

        [JsonProperty("api_stype")]
        public int TypeId { get; set; }
        [JsonProperty("api_ctype")]
        public int ClassId { get; set; }

        [JsonProperty("api_afterlv")]
        public int UpgradeLevel { get; set; }
        [JsonProperty("api_aftershipid")]
        public int UpgradeTo { get; set; }
        public int api_afterfuel;
        public int api_afterbull;
        public Materials UpgradeConsumption => new Materials
        {
            Bullet = api_afterbull,
            Steel = api_afterfuel
        };

        public IReadOnlyCollection<ItemRecord> UpgradeSpecialConsumption { get; set; } = Array.Empty<ItemRecord>();

        private static ShipMordenizationStatus MordenizeFromArray(int[] array)
            => new ShipMordenizationStatus(array.ElementAtOrDefault(0), array.ElementAtOrDefault(1), 0);

        public int[] api_taik;
        public ShipMordenizationStatus HP => MordenizeFromArray(api_taik);

        public int[] api_souk;
        public ShipMordenizationStatus Armor => MordenizeFromArray(api_souk);

        public int[] api_houg;
        public ShipMordenizationStatus Firepower => MordenizeFromArray(api_houg);

        public int[] api_raig;
        public ShipMordenizationStatus Torpedo => MordenizeFromArray(api_raig);

        public int[] api_tyku;
        public ShipMordenizationStatus AntiAir => MordenizeFromArray(api_tyku);

        public int[] api_tais;
        public ShipMordenizationStatus AntiSubmarine => MordenizeFromArray(api_tais);

        public int[] api_luck;
        public ShipMordenizationStatus Luck => MordenizeFromArray(api_luck);

        [JsonProperty("api_soku")]
        public ShipSpeed Speed { get; set; }
        [JsonProperty("api_leng")]
        public FireRange FireRange { get; set; }

        [JsonProperty("api_slot_num")]
        public int SlotCount { get; set; }
        [JsonProperty("api_maxeq")]
        public IReadOnlyList<int> Aircraft { get; set; }

        public int api_buildtime;
        public TimeSpan ConstructionTime => TimeSpan.FromMinutes(api_buildtime);

        public int[] api_broken;
        public Materials DismantleAcquirement => new Materials
        {
            Fuel = api_broken.ElementAtOrDefault(0),
            Bullet = api_broken.ElementAtOrDefault(1),
            Steel = api_broken.ElementAtOrDefault(2),
            Bauxite = api_broken.ElementAtOrDefault(3)
        };

        [JsonProperty("api_powerup")]
        public IReadOnlyList<int> PowerupWorth { get; set; }

        [JsonProperty("api_fuel_max")]
        public int FuelConsumption { get; set; }
        [JsonProperty("api_bull_max")]
        public int BulletConsumption { get; set; }
    }
}
