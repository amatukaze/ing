using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawRequiredInfo
    {
        [JsonProperty("api_basic")]
        public RawBasic Admiral { get; set; }

        [JsonProperty("api_slot_item")]
        public RawEquipment[] Equipment { get; set; }

        [JsonProperty("api_kdock")]
        public RawConstructionDock[] ConstructionDocks { get; set; }

        //[JsonProperty("api_useitem")]
    }
}
