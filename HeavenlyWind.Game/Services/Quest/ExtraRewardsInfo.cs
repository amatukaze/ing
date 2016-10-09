using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class ExtraRewardsInfo
    {
        [JsonProperty("materials")]
        public MaterialsReward Materials { get; internal set; }

        [JsonProperty("equipment")]
        public EquipmentReward[] Equipment { get; internal set; }

        [JsonProperty("items")]
        public ItemReward[] Items { get; internal set; }

        public class MaterialsReward : ModelBase
        {
            [JsonProperty("ic")]
            public int InstantConstruction { get; internal set; }

            [JsonProperty("bucket")]
            public int Bucket { get; internal set; }

            [JsonProperty("dm")]
            public int DevelopmentMaterial { get; internal set; }

            [JsonProperty("im")]
            public int ImprovementMaterial { get; internal set; }
        }
        public class EquipmentReward : ModelBase
        {
            [JsonProperty("id")]
            public int ID { get; internal set; }

            [JsonProperty("count")]
            public int Count { get; internal set; }
        }
        public class ItemReward : ModelBase
        {
            [JsonProperty("id")]
            public int ID { get; internal set; }

            [JsonProperty("count")]
            public int Count { get; internal set; }
        }
    }
}
