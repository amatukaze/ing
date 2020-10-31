using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawSlotItemInfo
    {
        [JsonPropertyName("api_id")]
        public SlotItemInfoId Id { get; set; }

        [JsonPropertyName("api_name")]
        public string Name { get; set; }

        [JsonPropertyName("api_info")]
        public string Description { get; set; }

        public int[] api_type { get; set; }
        public SlotItemTypeId TypeId => (SlotItemTypeId)api_type[2];
        public int IconId => api_type[3];
        public int PlaneId => api_type[4];

        [JsonPropertyName("api_houg")]
        /// <summary>火力</summary>
        public int Firepower { get; set; }
        [JsonPropertyName("api_raig")]
        /// <summary>雷装</summary>
        public int Torpedo { get; set; }
        [JsonPropertyName("api_tyku")]
        /// <summary>対空</summary>
        public int AntiAir { get; set; }
        [JsonPropertyName("api_souk")]
        /// <summary>装甲</summary>
        public int Armor { get; set; }
        [JsonPropertyName("api_baku")]
        /// <summary>爆装</summary>
        public int DiveBomberAttack { get; set; }
        [JsonPropertyName("api_tais")]
        /// <summary>対潜</summary>
        public int AntiSubmarine { get; set; }

        public int api_houm { get; set; }
        public int api_houk { get; set; }
        /// <summary>命中</summary>
        public int Accuracy => TypeId != 48 ? api_houm : 0;
        /// <summary>回避</summary>
        public int Evasion => TypeId != 48 ? api_houk : 0;
        /// <summary>対爆</summary>
        public int AntiBomber => TypeId == 48 ? api_houm : 0;
        /// <summary>迎撃</summary>
        public int Interception => TypeId == 48 ? api_houk : 0;

        [JsonPropertyName("api_saku")]
        /// <summary>索敵</summary>
        public int LineOfSight { get; set; }

        [JsonPropertyName("api_length")]
        public FireRange FireRange { get; set; }

        [JsonPropertyName("api_distance")]
        public int FlightRadius { get; set; }

        public int api_cost { get; set; }
        public Materials DeploymentConsumption => new Materials() { Bauxite = api_cost };

        [JsonPropertyName("api_broken")]
        public Materials DismantleAcquirement { get; set; }

        [JsonPropertyName("api_rare")]
        public int Rarity { get; set; }
    }
#nullable enable
}
