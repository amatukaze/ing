using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class ScreenshotPreference
    {
        [JsonProperty("destination")]
        public Property<string> Destination { get; private set; } = new Property<string>("Screenshot");
        [JsonProperty("filenameformat")]
        public Property<string> FilenameFormat { get; private set; } = new Property<string>("ing_{0:yyyyMMdd-HHmmss-ff}.{1}");
        [JsonProperty("imageformat")]
        public Property<ScreenshotImageFormat> ImageFormat { get; private set; } = new Property<ScreenshotImageFormat>(ScreenshotImageFormat.Png);
    }
}
