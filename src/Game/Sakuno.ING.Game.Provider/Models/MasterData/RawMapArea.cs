using Sakuno.ING.Game.Json.Converters;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawMapArea : IIdentifiable<MapAreaId>
    {
        [JsonPropertyName("api_id")]
        public MapAreaId Id { get; set; }
        [JsonPropertyName("api_name")]
        public string Name { get; set; }
        [JsonPropertyName("api_type")]
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool IsEventArea { get; set; }
    }
#nullable enable
}
