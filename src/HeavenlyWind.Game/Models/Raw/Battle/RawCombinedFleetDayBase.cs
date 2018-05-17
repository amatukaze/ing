using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public abstract class RawCombinedFleetDayBase : RawDay, IRawCombinedFleet
    {
        [JsonProperty("api_f_nowhps_combined")]
        public int[] FriendEscortCurrentHPs { get; set; }
        [JsonProperty("api_f_maxhps_combined")]
        public int[] FriendEscortMaximumHPs { get; set; }

        [JsonProperty("api_e_nowhps_combined")]
        public int[] EnemyEscortCurrentHPs { get; set; }
        [JsonProperty("api_e_maxhps_combined")]
        public int[] EnemyEscortMaximumHPs { get; set; }

        [JsonProperty("api_hougeki3")]
        public RawShellingPhase ShellingThirdRound { get; set; }
    }
}
