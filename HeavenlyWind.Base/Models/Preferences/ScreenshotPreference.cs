using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class ScreenshotPreference : ModelBase
    {
        string r_Destination = "Screenshot";
        [JsonProperty("destination")]
        public string Destination
        {
            get { return r_Destination; }
            set
            {
                r_Destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }
        [JsonProperty("filenameformat")]
        public string FilenameFormat { get; set; } = "ing_{0:yyyyMMdd-HHmmss-ff}.{1}";
        [JsonProperty("imageformat")]
        public ScreenshotImageFormat ImageFormat { get; set; } = ScreenshotImageFormat.Png;
    }
}
