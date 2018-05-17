using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEventMapDifficultySelectionResult
    {
        [JsonProperty("api_max_maphp")]
        public int? MaxHP { get; set; }
    }
}
