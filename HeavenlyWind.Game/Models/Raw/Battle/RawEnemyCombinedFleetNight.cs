using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawEnemyCombinedFleetNight : RawCombinedFleetNightBase
    {
        [JsonProperty("api_active_deck")]
        public int[] ParticipatingFleet { get; set; }
    }
}
