using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawAirForceGroupCombatRadius
    {
        [JsonProperty("api_base")]
        public int Base { get; set; }

        [JsonProperty("api_bonus")]
        public int Bonus { get; set; }
    }
}
