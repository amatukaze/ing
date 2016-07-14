using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class GamePreference
    {
        [JsonProperty("main_los_formula")]
        public Property<FleetLoSFormula> MainFleetLoSFormula { get; private set; } = new Property<FleetLoSFormula>(FleetLoSFormula.Formula33);

        [JsonProperty("main_fp_formula")]
        public Property<FleetFighterPowerFormula> MainFighterPowerFormula { get; private set; } = new Property<FleetFighterPowerFormula>(FleetFighterPowerFormula.WithBonus);

        [JsonProperty("fatigue_ceiling")]
        public Property<int> FatigueCeiling { get; private set; } = new Property<int>(40);

        [JsonProperty("show_battle_info")]
        public Property<bool> ShowBattleInfo { get; private set; } = new Property<bool>(true);

        [JsonProperty("show_drop")]
        public Property<bool> ShowDrop { get; private set; } = new Property<bool>(true);
    }
}
