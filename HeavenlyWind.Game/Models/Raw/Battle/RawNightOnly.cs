using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawNightOnly : RawNight, IRawFormationAndEngagementForm
    {
        [JsonProperty("api_formation")]
        public int[] FormationAndEngagementForm { get; set; }
    }
}
