using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    internal class UseItemCountJson : IRawUseItemCount
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_count")]
        public int Count { get; set; }
    }
}
