using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementAssistant
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("days")]
        public int Days { get; set; }
    }
}
