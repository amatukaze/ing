using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class GamePreference
    {
        [JsonProperty("main_los_formula")]
        public int MainFleetLoSFormula { get; set; } = 3;
    }
}
