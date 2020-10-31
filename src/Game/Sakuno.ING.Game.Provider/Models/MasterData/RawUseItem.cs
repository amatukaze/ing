using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawUseItem
    {
        [JsonPropertyName("api_id")]
        public UseItemId Id { get; set; }
        [JsonPropertyName("api_name")]
        public string Name { get; set; }
    }
#nullable enable
}
