using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawImprovementResult
    {
        [JsonProperty("api_remodel_flag")]
        public bool Success { get; set; }

        [JsonProperty("api_remodel_id")]
        public int[] MasterID { get; set; }

        [JsonProperty("api_after_material")]
        public int[] Materials { get; set; }

        //[JsonProperty("api_voice_ship_id")]
        //[JsonProperty("api_voice_id")]

        [JsonProperty("api_after_slot")]
        public RawEquipment ImprovedEquipment { get; set; }

        [JsonProperty("api_use_slot_id")]
        public int? RemovedEquipmentID { get; set; }
    }
}
