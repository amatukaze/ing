using Newtonsoft.Json;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class WindowPreference
    {
        public string Name { get; }

        [JsonProperty("state")]
        public WindowState State { get; set; }

        [JsonProperty("left")]
        public double Left { get; set; }
        [JsonProperty("top")]
        public double Top { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }
        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("topmost")]
        public bool TopMost { get; set; }

        public WindowPreference(string rpName)
        {
            Name = rpName;
        }
    }
}
