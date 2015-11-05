using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawQuest : IID
    {
        [JsonProperty("api_no")]
        public int ID { get; set; }

        [JsonProperty("api_category")]
        public QuestCategory Category { get; set; }
        [JsonProperty("api_type")]
        public QuestType Type { get; set; }

        [JsonProperty("api_state")]
        public QuestState State { get; set; }

        [JsonProperty("api_title")]
        public string Name { get; set; }
        [JsonProperty("api_detail")]
        public string Description { get; set; }

        [JsonProperty("api_get_material")]
        public int[] RewardMaterials { get; set; }

        [JsonProperty("api_bonus_flag")]
        public int BonusType { get; set; }

        [JsonProperty("api_progress_flag")]
        public QuestProgress Progress { get; set; }

        [JsonProperty("api_invalid_flag")]
        public bool CannotCompleted { get; set; }

    }
}
