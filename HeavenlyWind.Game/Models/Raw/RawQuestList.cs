using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public JToken Quests { get; set; }

        [JsonProperty("api_exec_count")]
        public int ExecutingCount { get; set; }

        [JsonProperty("api_exec_type")]
        public int ExecutingType { get; set; }
    }
}
