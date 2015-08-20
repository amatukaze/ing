using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class CachePreference
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty("folder")]
        public string CacheFolder { get; set; } = "Cache";
    }
}
