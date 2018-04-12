using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class BgmInfoJson : IRawBgmInfo
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
    }
}
