using Newtonsoft.Json;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze
{
    public class ExtraStringResourceInfo
    {
        public static ExtraStringResourceInfo Disabled { get; } = new ExtraStringResourceInfo() { Directory = null, DisplayName = "Disabled" };

        [JsonIgnore]
        public string Directory { get; internal set; }

        [JsonProperty("culture")]
        public string CultureName { get; internal set; }
        [JsonProperty("name")]
        public string DisplayName { get; internal set; }

        [JsonProperty("contents")]
        public ContentInfo[] Contents { get; internal set; }

        public class ContentInfo
        {
            [JsonProperty("type")]
            public ExtraStringResourceType Type { get; internal set; }

            [JsonProperty("version")]
            public int Version { get; internal set; }

            [JsonIgnore]
            public FileInfo File { get; internal set; }
        }
    }
}
