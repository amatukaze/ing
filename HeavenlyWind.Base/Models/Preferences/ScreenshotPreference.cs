using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class ScreenshotPreference
    {
        [JsonProperty("folder")]
        public string Folder { get; set; } = "Screenshot";
        [JsonProperty("filenameformat")]
        public string FilenameFormat { get; set; } = "hw_{0:yyyyMMdd-HHmmss-ff}.{1}";
        [JsonProperty("imageformat")]
        public ScreenshotImageFormat ImageFormat { get; set; } = ScreenshotImageFormat.Png;
    }
}
