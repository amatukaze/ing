using Sakuno.ING.Game.Json.Converters;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
#nullable disable
    public sealed class RawFleet : IIdentifiable<FleetId>
    {
        [JsonPropertyName("api_id")]
        public FleetId Id { get; set; }
        [JsonPropertyName("api_name")]
        public string Name { get; set; }

        [JsonPropertyName("api_mission")]
        [JsonConverter(typeof(FleetExpeditionStatusConverter))]
        public RawFleetExpeditionStatus ExpeditionStatus { get; set; }

        [JsonPropertyName("api_ship")]
        public ShipId[] ShipIds { get; set; }
    }
#nullable enable
}
