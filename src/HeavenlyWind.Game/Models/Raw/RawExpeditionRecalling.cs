using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawExpeditionRecalling
    {
        [JsonProperty("api_mission")]
        public long[] Expedition { get; set; }
    }
}
