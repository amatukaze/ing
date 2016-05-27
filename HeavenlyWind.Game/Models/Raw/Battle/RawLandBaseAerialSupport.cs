using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    public class RawLandBaseAerialSupport : RawAerialCombatPhase
    {
        [JsonProperty("api_base_id")]
        public int LandBaseID { get; set; }

        //[JsonProperty("api_stage_flag")]

        [JsonProperty("api_squadron_plane")]
        public RawParticipant[] Participants { get; set; }

        public class RawParticipant
        {
            [JsonProperty("api_mst_id")]
            public int PlaneID { get; set; }

            [JsonProperty("api_count")]
            public int Count { get; set; }
        }
    }
}
