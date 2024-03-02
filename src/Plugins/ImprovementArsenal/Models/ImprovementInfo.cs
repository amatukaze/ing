using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementInfo
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("resources")]
        public ImprovementResourceConsumption Resources { get; set; }

        [JsonProperty("initial")]
        public ImprovementStage InitialStage { get; set; }

        [JsonProperty("middle")]
        public ImprovementStage MiddleStage { get; set; }

        [JsonProperty("branches")]
        public ImprovementBranch[] Branches { get; set; }

        [JsonProperty("assistants")]
        public ImprovementAssistant[] Assistants { get; private set; }
    }
}
