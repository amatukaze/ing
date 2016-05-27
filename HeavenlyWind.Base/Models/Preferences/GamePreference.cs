using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class GamePreference
    {
        [JsonProperty("main_los_formula")]
        public int MainFleetLoSFormula { get; set; } = 3;

        [JsonProperty("fatigue_ceiling")]
        public int FatigueCeiling { get; set; } = 40;
    }
}
