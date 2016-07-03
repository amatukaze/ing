using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class BrowserPreference : ModelBase
    {
        [JsonProperty("zoom")]
        public double Zoom { get; set; } = 1.0;

        [JsonProperty("homepage")]
        public string Homepage { get; set; } = "http://www.dmm.com/netgame/social/application/-/detail/=/app_id=854854/";

        [JsonProperty("layoutengine")]
        public string CurrentLayoutEngine { get; set; } = "trident";

        [JsonProperty("flash")]
        public FlashPreference Flash { get; set; } = new FlashPreference();

        [JsonProperty("screenshot")]
        public ScreenshotPreference Screenshot { get; set; } = new ScreenshotPreference();

        bool r_ActionBarPlacement;
        [JsonProperty("action_bar_placement")]
        public bool ActionBarPlacement
        {
            get { return r_ActionBarPlacement; }
            set
            {
                if (r_ActionBarPlacement != value)
                {
                    r_ActionBarPlacement = value;
                    OnPropertyChanged(nameof(ActionBarPlacement));
                }
            }
        }

        bool r_ActionBarStickToBrowser;
        [JsonProperty("action_bar_stick_to_browser")]
        public bool ActionBarStickToBrowser
        {
            get { return r_ActionBarStickToBrowser; }
            set
            {
                if (r_ActionBarStickToBrowser != value)
                {
                    r_ActionBarStickToBrowser = value;
                    OnPropertyChanged(nameof(ActionBarStickToBrowser));
                }
            }
        }
    }
}
