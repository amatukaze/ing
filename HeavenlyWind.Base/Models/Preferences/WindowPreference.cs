using Newtonsoft.Json;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class WindowPreference
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public WindowState State { get; set; }

        [JsonProperty("left")]
        public int Left { get; set; }
        [JsonProperty("top")]
        public int Top { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
