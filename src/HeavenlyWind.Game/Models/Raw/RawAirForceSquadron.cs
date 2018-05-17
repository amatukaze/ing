using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawAirForceSquadron : IID
    {
        [JsonProperty("api_squadron_id")]
        public int ID { get; set; }

        [JsonProperty("api_state")]
        public AirForceSquadronState State { get; set; }

        [JsonProperty("api_slotid")]
        public int EquipmentID { get; set; }

        [JsonProperty("api_count")]
        public int Count { get; set; }
        [JsonProperty("api_max_count")]
        public int MaxCount { get; set; }

        [JsonProperty("api_cond")]
        public AirForceSquadronCondition Condition { get; set; }
    }
}
