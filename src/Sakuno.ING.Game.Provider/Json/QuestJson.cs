using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class QuestJson : IRawQuest
    {
        [JsonProperty("api_no")]
        public int Id { get; set; }

        public int api_category;
        public QuestCategoty Category => api_category == 8 ?
            QuestCategoty.Sortie : (QuestCategoty)api_category;

        public int api_type;
        public QuestPeriod Period =>
            api_type == 218 || api_type == 211 ?
            QuestPeriod.Daily : (QuestPeriod)api_type;

        [JsonProperty("api_state")]
        public QuestState State { get; set; }
        [JsonProperty("api_title")]
        public string Name { get; set; }
        [JsonProperty("api_detail"), JsonConverter(typeof(HtmlNewLineEater))]
        public string Description { get; set; }
        [JsonProperty("api_get_material"), JsonConverter(typeof(MaterialsConverter))]
        public Materials Rewards { get; set; }
        [JsonProperty("api_progress_flag")]
        public QuestProgress Progress { get; set; }
    }
}
