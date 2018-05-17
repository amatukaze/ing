using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawCombinedFleetNightOnly : RawCombinedFleetNight, IRawFormationAndEngagementForm, ISupportingFire
    {
        [JsonProperty("api_formation")]
        public int[] FormationAndEngagementForm { get; set; }

        [JsonProperty("api_n_support_flag")]
        public int SupportingFireType { get; set; }
        [JsonProperty("api_n_support_info")]
        public RawSupportingFirePhase SupportingFire { get; set; }
    }
}
