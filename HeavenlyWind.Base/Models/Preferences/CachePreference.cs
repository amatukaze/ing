using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class CachePreference
    {
        [JsonProperty("mode")]
        public CacheMode Mode { get; set; } = CacheMode.Disabled;

        [JsonProperty("path")]
        public string Path { get; set; } = "Cache";
    }
}
