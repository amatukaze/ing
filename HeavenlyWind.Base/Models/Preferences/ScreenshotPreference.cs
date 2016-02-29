using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class ScreenshotPreference
    {
        [JsonProperty("destination")]
        public string Destination { get; set; } = "Screenshot";
        [JsonProperty("filenameformat")]
        public string FilenameFormat { get; set; } = "ing_{0:yyyyMMdd-HHmmss-ff}.{1}";
        [JsonProperty("imageformat")]
        public ScreenshotImageFormat ImageFormat { get; set; } = ScreenshotImageFormat.Png;
    }
}
