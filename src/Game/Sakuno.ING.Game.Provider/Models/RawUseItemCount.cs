using Sakuno.ING.Game.Models.MasterData;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawUseItemCount
    {
        [JsonPropertyName("api_id")]
        public UseItemId Id { get; set; }
        [JsonPropertyName("api_count")]
        public int Count { get; set; }
    }
}
