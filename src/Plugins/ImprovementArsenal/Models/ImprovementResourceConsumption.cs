using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models
{
    class ImprovementResourceConsumption : ModelBase
    {
        [JsonProperty("fuel")]
        public int Fuel { get; private set; }

        [JsonProperty("bullet")]
        public int Bullet { get; private set; }

        [JsonProperty("steel")]
        public int Steel { get; private set; }

        [JsonProperty("bauxite")]
        public int Bauxite { get; private set; }
    }
}
