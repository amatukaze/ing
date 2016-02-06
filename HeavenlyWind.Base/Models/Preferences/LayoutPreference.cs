using Newtonsoft.Json;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class LayoutPreference
    {
        [JsonProperty("landscape")]
        public Dock LandscapeDock { get; set; } = Dock.Left;

        [JsonProperty("portrait")]
        public Dock PortraitDock { get; set; } = Dock.Top;
    }
}
