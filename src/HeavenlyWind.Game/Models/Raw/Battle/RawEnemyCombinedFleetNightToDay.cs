using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    class RawEnemyCombinedFleetNightToDay : RawEnemyCombinedFleetDay
    {
        [JsonProperty("api_friendly_battle")]
        public RawNpcSupportingFire NpcSupportingFire { get; set; }

        [JsonProperty("api_n_support_flag")]
        public int NightSupportingFireType { get; set; }
        [JsonProperty("api_n_support_info")]
        public RawSupportingFirePhase NightSupportingFire { get; set; }

        [JsonProperty("api_n_hougeki1")]
        public RawShellingPhase NightShellingFirstRound { get; set; }
        [JsonProperty("api_n_hougeki2")]
        public RawShellingPhase NightShellingSecondRound { get; set; }

        public class RawNpcSupportingFire
        {
            [JsonProperty("api_hougeki")]
            public RawShellingPhase Shelling { get; set; }
        }
    }
}
