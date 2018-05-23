using Newtonsoft.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.MasterData
{
    internal class MapAreaJson : IRawMapArea
    {
        [JsonProperty("api_id")]
        public MapAreaId Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_type")]
        public bool IsEvent { get; set; }
    }
}
