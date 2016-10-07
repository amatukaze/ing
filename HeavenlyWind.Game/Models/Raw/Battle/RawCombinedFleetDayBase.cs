using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public abstract class RawCombinedFleetDayBase : RawDay, IRawCombinedFleet
    {
        [JsonProperty("api_nowhps_combined")]
        public int[] EscortFleetCurrentHPs { get; set; }
        [JsonProperty("api_maxhps_combined")]
        public int[] EscortFleetMaximumHPs { get; set; }

        [JsonProperty("api_hougeki3")]
        public RawShellingPhase ShellingThirdRound { get; set; }
    }
}
