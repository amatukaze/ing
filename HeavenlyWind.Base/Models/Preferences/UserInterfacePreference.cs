using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UserInterfacePreference
    {
        [JsonProperty("font")]
        public Property<string> Font { get; private set; } = new UIFontProperty();

        [JsonProperty("zoom")]
        public Property<double> Zoom { get; private set; } = new Property<double>(1.0);

        [JsonProperty("hd_line")]
        public HeavyDamageLinePreference HeavyDamageLine { get; private set; } = new HeavyDamageLinePreference();

        [JsonProperty("use_game_material_icons")]
        public Property<bool> UseGameMaterialIcons { get; private set; } = new Property<bool>();
    }
}
