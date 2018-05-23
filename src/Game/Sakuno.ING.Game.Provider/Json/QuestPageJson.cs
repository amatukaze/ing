using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;

namespace Sakuno.ING.Game.Json
{
    internal class QuestPageJson
    {
        public int api_count;
        public bool api_completed_kind;
        public int api_page_count;
        public int api_disp_page;
        [JsonConverter(typeof(Minus1Eater<QuestJson>))]
        public IReadOnlyList<QuestJson> api_list;
        public int api_exec_count;
    }
}
