using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawAirForceGroupOrganization
    {
        [JsonProperty("api_distance")]
        public int CombatRadius { get; set; }

        [JsonProperty("api_plane_info")]
        public RawAirForceSquadron[] Squadrons { get; set; }

        [JsonProperty("api_after_bauxite")]
        public int? Bauxite { get; set; }
    }
}
