using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class EquipmentInfoJson : IRawEquipmentInfo
    {
        [JsonProperty("api_id")]
        public EquipmentInfoId Id { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_info"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; set; }

        public int[] api_type;
        public EquipmentTypeId TypeId => (EquipmentTypeId)api_type.ElementAtOrDefault(2);
        public int IconId => api_type.ElementAtOrDefault(3);

        public IReadOnlyCollection<ShipInfoId> ExtraSlotAcceptingShips { get; set; } = Array.Empty<ShipInfoId>();

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
        public int api_houk;
        public int Accuracy => TypeId != 48 ? api_houm : 0;
        public int Evasion => TypeId != 48 ? api_houk : 0;
        public int AntiBomber => TypeId == 48 ? api_houm : 0;
        public int Interception => TypeId == 48 ? api_houk : 0;

        [JsonProperty("api_saku")]
        public int LineOfSight { get; set; }

        [JsonProperty("api_length")]
        public FireRange FireRange { get; set; }

        [JsonProperty("api_distance")]
        public int FlightRadius { get; set; }

        public int api_cost;
        public Materials DeploymentConsumption => new Materials { Bauxite = api_cost };

        [JsonProperty("api_broken"), JsonConverter(typeof(MaterialsConverter))]
        public Materials DismantleAcquirement { get; set; }

        [JsonProperty("api_rare")]
        public int Rarity { get; set; }
    }
}
