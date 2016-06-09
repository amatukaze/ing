using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class CachePreference : ModelBase
    {
        [JsonProperty("mode")]
        public CacheMode Mode { get; set; }

        string r_Path = "Cache";
        [JsonProperty("path")]
        public string Path
        {
            get { return r_Path; }
            set
            {
                if (r_Path != value)
                {
                    r_Path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

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
