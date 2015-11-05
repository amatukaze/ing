using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;

namespace Sakuno.KanColle.Amatsukaze
{
    partial class Preference
    {

        [JsonProperty("version")]
        public string Version { get; } = ProductInfo.AssemblyVersionString;

        [JsonProperty("language")]
        public string Language { get; set; } = StringResources.GetDefaultLanguage();

        [JsonProperty("firstrun")]
        public bool FirstRun { get; set; } = true;

        [JsonProperty("checkupdate")]
        public bool CheckUpdate { get; set; } = true;
        [JsonProperty("channel")]
        public UpdateChannel UpdateChannel { get; set; } = UpdateChannel.Release;

        [JsonProperty("network")]
        public NetworkPreference Network { get; set; } = new NetworkPreference();

        [JsonProperty("cache")]
        public CachePreference Cache { get; set; } = new CachePreference();

        [JsonProperty("browser")]
        public BrowserPreference Browser { get; set; } = new BrowserPreference();

        [JsonProperty("windows")]
        public WindowPreference[] Windows { get; set; }

    }
}
