using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class FlashPreference
    {
        [JsonProperty("quality")]
        public FlashQuality Quality { get; set; } = FlashQuality.Default;

        [JsonProperty("rendermode")]
        public FlashRenderMode RenderMode { get; set; } = FlashRenderMode.Default;
    }
}
