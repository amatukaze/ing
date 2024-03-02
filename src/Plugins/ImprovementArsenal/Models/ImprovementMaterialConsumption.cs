using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementMaterialConsumption : ModelBase
    {
        [JsonProperty("base")]
        public int WithoutGS { get; private set; }

        [JsonProperty("gs")]
        public int WithGS { get; private set; }
    }
}
