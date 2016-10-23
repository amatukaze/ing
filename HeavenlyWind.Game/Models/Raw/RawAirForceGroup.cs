using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawAirForceGroup : IID
    {
        [JsonProperty("api_area_id")]
        public int AreaID { get; set; }

        [JsonProperty("api_rid")]
        public int ID { get; set; }

        [JsonProperty("api_name")]
        public string Name { get; set; }

        [JsonProperty("api_distance")]
        public int CombatRadius { get; set; }

        [JsonProperty("api_action_kind")]
        public AirForceGroupOption Option { get; set; }

        [JsonProperty("api_plane_info")]
        public RawAirForceSquadron[] Squadrons { get; set; }
    }
}
