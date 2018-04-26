using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class EquipmentJson : IRawEquipment
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_slotitem_id")]
        public int EquipmentInfoId { get; set; }
        [JsonProperty("api_locked")]
        public bool IsLocked { get; set; }
        [JsonProperty("api_level")]
        public int ImprovementLevel { get; set; }
        [JsonProperty("api_alv")]
        public int AirProficiency { get; set; }
    }
}
