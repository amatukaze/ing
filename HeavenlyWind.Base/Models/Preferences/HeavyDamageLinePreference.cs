using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class HeavyDamageLinePreference
    {
        [JsonProperty("color")]
        public Property<HeavyDamageLineType> Type { get; private set; } = new Property<HeavyDamageLineType>(HeavyDamageLineType.Default);

        [JsonProperty("width")]
        public Property<int> Width { get; private set; } = new HeavyDamageLineWidthProperty();
    }
}
