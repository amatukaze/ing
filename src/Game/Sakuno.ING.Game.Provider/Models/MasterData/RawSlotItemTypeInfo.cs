using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable

    public sealed class RawSlotItemTypeInfo
    {
        [JsonPropertyName("api_id")]
        public SlotItemTypeId Id { get; set; }
        [JsonPropertyName("api_name")]
        public string Name { get; set; }
    }
#nullable enable
}
