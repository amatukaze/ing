using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class BrowserPreference
    {
        [JsonProperty("zoom")]
        public Property<double> Zoom { get; private set; } = new Property<double>(1.0);

        [JsonProperty("homepage")]
        public Property<string> Homepage { get; private set; } = new Property<string>("http://www.dmm.com/netgame/social/application/-/detail/=/app_id=854854/");

        [JsonProperty("layoutengine")]
        public Property<string> CurrentLayoutEngine { get; private set; } = new Property<string>("trident");

        [JsonProperty("flash")]
        public FlashPreference Flash { get; private set; } = new FlashPreference();

        [JsonProperty("screenshot")]
        public ScreenshotPreference Screenshot { get; private set; } = new ScreenshotPreference();

        [JsonProperty("action_bar_placement")]
        public Property<bool> ActionBarPlacement { get; private set; } = new Property<bool>();

        [JsonProperty("action_bar_stick_to_browser")]
        public Property<bool> ActionBarStickToBrowser { get; private set; } = new Property<bool>();
    }
}
