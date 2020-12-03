using Sakuno.ING.Game.Models.MasterData;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
#nullable disable
    public sealed class RawMap : IIdentifiable<MapId>
    {
        [JsonPropertyName("api_id")]
        public MapId Id { get; set; }

        [JsonPropertyName("api_cleared")]
        public bool IsCleared { get; set; }

        [JsonPropertyName("api_air_base_decks")]
        public int AvailableAirForceGroups { get; set; }

        [JsonPropertyName("api_defeat_count")]
        public int? DefeatedCount { get; set; }
        [JsonPropertyName("api_required_defeat_count")]
        public int? RequiredDefeatCount { get; set; }
    }
#nullable enable
}
