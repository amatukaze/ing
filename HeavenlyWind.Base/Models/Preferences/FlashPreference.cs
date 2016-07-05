using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class FlashPreference
    {
        [JsonProperty("quality")]
        public Property<FlashQuality> Quality { get; private set; } = new Property<FlashQuality>(FlashQuality.Default);

        [JsonProperty("rendermode")]
        public Property<FlashRenderMode> RenderMode { get; private set; } = new Property<FlashRenderMode>(FlashRenderMode.Default);
    }
}
