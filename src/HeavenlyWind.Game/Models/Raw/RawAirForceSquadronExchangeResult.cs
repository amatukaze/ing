using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawAirForceSquadronExchangeResult
    {
        [JsonProperty("api_base_items")]
        public RawAirForceGroup[] Groups { get; set; }
    }
}
