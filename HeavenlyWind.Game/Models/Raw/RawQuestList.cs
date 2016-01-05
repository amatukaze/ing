using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawQuestList
    {
        [JsonProperty("api_count")]
        public int Count { get; set; }

        [JsonProperty("api_page_count")]
        public int PageCount { get; set; }

        [JsonProperty("api_disp_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("api_list")]
        public JToken QuestsJson { get; set; }

        [JsonProperty("api_exec_count")]
        public int ExecutingCount { get; set; }

        [JsonProperty("api_exec_type")]
        public int ExecutingType { get; set; }

        [JsonIgnore]
        public RawQuest[] Quests { get; internal set; }
    }
}
