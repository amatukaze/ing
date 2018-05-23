using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal class UseItemCountJson : IRawUseItemCount
    {
        [JsonProperty("api_id")]
        public UseItemId Id { get; set; }
        [JsonProperty("api_count")]
        public int Count { get; set; }
    }
}
