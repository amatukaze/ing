using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models.MasterData;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawSlotItem
    {
        [JsonPropertyName("api_id")]
        public SlotItemId Id { get; set; }
        [JsonPropertyName("api_slotitem_id")]
        public SlotItemInfoId SlotItemInfoId { get; set; }
        [JsonPropertyName("api_locked")]
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool IsLocked { get; set; }
        [JsonPropertyName("api_level")]
        public int ImprovementLevel { get; set; }
        [JsonPropertyName("api_alv")]
        public int AerialProficiency { get; set; }
    }
}
