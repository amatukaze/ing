using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementStage : ModelBase
    {
        [JsonProperty("dm")]
        public ImprovementMaterialConsumption DevelopmentMaterialConsumption { get; private set; }

        [JsonProperty("im")]
        public ImprovementMaterialConsumption ImprovementMaterialConsumption { get; private set; }

        [JsonProperty("consumption")]
        public ImprovementExtraConsumption[] ExtraConsumption { get; private set; }
    }
}
