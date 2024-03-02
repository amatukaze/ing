using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementBranch : ImprovementStage
    {
        [JsonProperty("destination")]
        public int Destination { get; private set; }

        [JsonProperty("level")]
        public int Level { get; private set; }

        [JsonProperty("assistants")]
        public ImprovementAssistant[] Assistants { get; private set; }
    }
}
