using Newtonsoft.Json;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class LayoutPreference : ModelBase
    {
        Dock r_LandscapeDock = Dock.Left;
        [JsonProperty("landscape")]
        public Dock LandscapeDock
        {
            get { return r_LandscapeDock; }
            set
            {
                if (r_LandscapeDock != value)
                {
                    r_LandscapeDock = value;
                    OnPropertyChanged(nameof(LandscapeDock));
                }
            }
        }

        Dock r_PortraitDock = Dock.Top;
        [JsonProperty("portrait")]
        public Dock PortraitDock
        {
            get { return r_PortraitDock; }
            set
            {
                if (r_PortraitDock != value)
                {
                    r_PortraitDock = value;
                    OnPropertyChanged(nameof(PortraitDock));
                }
            }
        }
    }
}
