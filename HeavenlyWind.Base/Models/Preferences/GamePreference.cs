using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class GamePreference
    {
        [JsonProperty("main_los_formula")]
        public int MainFleetLoSFormula { get; set; } = 3;

        [JsonProperty("fatigue_ceiling")]
        public int FatigueCeiling { get; set; } = 40;

        [JsonProperty("show_battle_info")]
        public bool ShowBattleInfo { get; set; } = true;

        [JsonProperty("show_drop")]
        public bool ShowDrop { get; set; } = true;
    }
}
