using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class ExtraRewardSelection
    {
        [JsonProperty("type")]
        public int Type { get; internal set; }

        [JsonProperty("id")]
        public int ID { get; internal set; }

        [JsonProperty("count")]
        public int Count { get; internal set; }
    }
}
