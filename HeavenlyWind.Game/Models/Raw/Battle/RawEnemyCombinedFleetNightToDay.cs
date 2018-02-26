using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    class RawEnemyCombinedFleetNightToDay : RawEnemyCombinedFleetDay
    {
        [JsonProperty("api_n_support_flag")]
        public int NightSupportingFireType { get; set; }
        [JsonProperty("api_n_support_info")]
        public RawSupportingFirePhase NightSupportingFire { get; set; }

        [JsonProperty("api_n_hougeki1")]
        public RawShellingPhase NightShellingFirstRound { get; set; }
        [JsonProperty("api_n_hougeki2")]
        public RawShellingPhase NightShellingSecondRound { get; set; }
    }
}
