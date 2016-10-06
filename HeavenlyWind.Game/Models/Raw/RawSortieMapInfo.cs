using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawSortieMapInfo
    {
        [JsonProperty("api_map_info")]
        public RawMapInfo[] Maps { get; set; }

        [JsonProperty("api_air_base")]
        public RawAirForceGroup[] AirForceGroups { get; set; }
    }
}
