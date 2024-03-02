using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementExtraConsumption : ModelBase
    {
        [JsonProperty("type")]
        public ExtraConsumptionType Type { get; private set; }

        [JsonProperty("id")]
        public int ID { get; private set; }

        [JsonProperty("count")]
        public int Count { get; private set; }
    }
}
