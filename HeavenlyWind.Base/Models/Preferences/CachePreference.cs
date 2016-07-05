using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class CachePreference
    {
        [JsonProperty("mode")]
        public Property<CacheMode> Mode { get; private set; } = new Property<CacheMode>();

        [JsonProperty("path")]
        public Property<string> Path { get; private set; } = new Property<string>("Cache");

        public CachePreference()
        {
            var rDirectory = new DirectoryInfo(Path);
            if (rDirectory.Exists && rDirectory.EnumerateFiles("*.swf", SearchOption.AllDirectories).Any())
                Mode.Value = CacheMode.FullTrust;
            else
                Mode.Value = CacheMode.Disabled;
        }
    }
}
