using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class UseItemJson : IRawUseItem
    {
        [JsonProperty("api_id")]
        public UseItemId Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
    }
}
