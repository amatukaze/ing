using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawAirForceSquadronResupplyResult
    {
        [JsonProperty("api_after_fuel")]
        public int Fuel { get; set; }

        [JsonProperty("api_after_bauxite")]
        public int Bauxite { get; set; }

        [JsonProperty("api_plane_info")]
        public RawAirForceSquadron[] Squadrons { get; set; }
    }
}
