using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class BrowserPreference
    {
        [JsonProperty("zoom")]
        public double Zoom { get; set; } = 1.0;

        [JsonProperty("homepage")]
        public string Homepage { get; set; } = "http://www.dmm.com/netgame/social/application/-/detail/=/app_id=854854/";
        [JsonProperty("game_token")]
        public string GameToken { get; set; }

        [JsonProperty("layoutengine")]
        public string CurrentLayoutEngine { get; set; } = "trident";

        [JsonProperty("flash")]
        public FlashPreference Flash { get; set; } = new FlashPreference();

        [JsonProperty("screenshot")]
        public ScreenshotPreference Screenshot { get; set; } = new ScreenshotPreference();
    }
}
