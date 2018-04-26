using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class UseItemCountJson : IRawUseItemCount
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_count")]
        public int Count { get; set; }
    }
}
