using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawAerialCombat : RawDay, IAerialCombatSecondRound
    {
        [JsonProperty("api_kouku2")]
        public RawAerialCombatPhase AerialCombatSecondRound { get; set; }
    }
}
