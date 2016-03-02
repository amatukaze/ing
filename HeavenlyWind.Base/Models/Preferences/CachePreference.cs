using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class CachePreference
    {
        [JsonProperty("mode")]
        public CacheMode Mode { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; } = "Cache";

        public CachePreference()
        {
            var rDirectory = new DirectoryInfo(Path);
            if (rDirectory.Exists && rDirectory.EnumerateFiles("*.swf", SearchOption.AllDirectories).Any())
                Mode = CacheMode.FullTrust;
            else
                Mode = CacheMode.Disabled;
        }
    }
}
