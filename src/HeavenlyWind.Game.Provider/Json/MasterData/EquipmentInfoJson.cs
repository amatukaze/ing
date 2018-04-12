using System;
using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class EquipmentInfoJson : IRawEquipmentInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_info", ItemConverterType = typeof(HtmlNewLineEater))]
        public string Description { get; set; }

        public int[] api_type;
        public int TypeId => api_type.ElementAtOrDefault(2);
        public int IconId => api_type.ElementAtOrDefault(3);

        public int[] ExtraSlotAcceptingShips { get; set; } = Array.Empty<int>();

        [JsonProperty("api_houg")]
        public int Firepower { get; set; }

        [JsonProperty("api_raig")]
        public int Torpedo { get; set; }

        [JsonProperty("api_tyku")]
        public int AntiAir { get; set; }

        [JsonProperty("api_souk")]
        public int Armor { get; set; }

        [JsonProperty("api_baku")]
        public int DiveBomberAttack { get; set; }

        [JsonProperty("api_tais")]
        public int AntiSubmarine { get; set; }

        public int api_houm;
        public int apu_houk;
        public int Accuracy => TypeId != 48 ? api_houm : 0;
        public int Evasion => TypeId != 48 ? apu_houk : 0;
        public int AntiBomber => TypeId == 48 ? api_houm : 0;
        public int Interception => TypeId == 48 ? apu_houk : 0;

        [JsonProperty("api_saku")]
        public int LightOfSight { get; set; }

        [JsonProperty("api_length")]
        public FireRange FireRange { get; set; }

        [JsonProperty("api_distance")]
        public int FlightRadius { get; set; }

        public int api_cost;
        public Materials DeploymentConsumption => new Materials { Bauxite = api_cost };

        public int[] api_broken;
        public Materials DismantleAcquirement => new Materials
        {
            Fuel = api_broken.ElementAtOrDefault(0),
            Bullet = api_broken.ElementAtOrDefault(1),
            Steel = api_broken.ElementAtOrDefault(2),
            Bauxite = api_broken.ElementAtOrDefault(3)
        };

        [JsonProperty("api_rare")]
        public int Rarity { get; set; }
    }
}
