using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipmentDevelopment
    {
        [JsonProperty("api_create_flag")]
        public bool Success { get; set; }

        [JsonProperty("api_slot_item")]
        public RawEquipmentDevelopmentResult Result { get; set; }

        [JsonProperty("api_material")]
        public int[] Materials { get; set; }

        [JsonProperty("api_type3")]
        public int EquipmentType { get; set; }

        public class RawEquipmentDevelopmentResult
        {
            [JsonProperty("api_id")]
            public int ID { get; set; }

            [JsonProperty("api_slotitem_id")]
            public int EquipmentID { get; set; }
        }
    }
}
