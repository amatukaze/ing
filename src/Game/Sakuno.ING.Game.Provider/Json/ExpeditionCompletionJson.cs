using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal class ExpeditionCompletionJson
    {
        public string api_quest_name;
        public ExpeditionResult api_clear_result;
        [JsonConverter(typeof(MaterialsConverter))]
        public Materials api_get_material;
        public ExpeditionItemJson api_get_item1;
        public ExpeditionItemJson api_get_item2;
        public IReadOnlyList<UseItemId> api_useitem_flag;
    }

    internal class ExpeditionItemJson
    {
        public UseItemId api_useitem_id;
        public int api_useitem_count;
    }
}
