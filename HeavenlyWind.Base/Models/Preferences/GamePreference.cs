using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class GamePreference
    {
        [JsonProperty("main_los_formula")]
        public FleetLoSFormula MainFleetLoSFormula { get; set; } = FleetLoSFormula.Formula33;

        [JsonProperty("main_fp_formula")]
        public FleetFighterPowerFormula MainFighterPowerFormula { get; set; } = FleetFighterPowerFormula.WithBonus;

        [JsonProperty("fatigue_ceiling")]
        public int FatigueCeiling { get; set; } = 40;

        [JsonProperty("show_battle_info")]
        public bool ShowBattleInfo { get; set; } = true;

        [JsonProperty("show_drop")]
        public bool ShowDrop { get; set; } = true;
    }
}
