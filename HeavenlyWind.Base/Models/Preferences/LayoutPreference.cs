using Newtonsoft.Json;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class LayoutPreference : ModelBase
    {
        [JsonProperty("landscape")]
        public Property<Dock> LandscapeDock { get; private set; } = new Property<Dock>(Dock.Left);

        [JsonProperty("portrait")]
        public Property<Dock> PortraitDock { get; private set; } = new Property<Dock>(Dock.Top);
    }
}
