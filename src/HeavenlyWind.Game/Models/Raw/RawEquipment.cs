using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw
{
    public class RawEquipment : IID
    {
        [JsonProperty("api_id")]
        public int ID { get; set; }

        [JsonProperty("api_slotitem_id")]
        public int EquipmentID { get; set; }

        [JsonProperty("api_locked")]
        public bool IsLocked { get; set; }

        [JsonProperty("api_level")]
        public int Level { get; set; }

        [JsonProperty("api_alv")]
        public int Proficiency { get; set; }
    }

}
