using System;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawRepairDock : IIdentifiable<RepairDockId>
    {
        [JsonPropertyName("api_id")]
        public RepairDockId Id { get; set; }

        [JsonPropertyName("api_state")]
        public RepairDockState State { get; set; }

        [JsonPropertyName("api_ship_id")]
        public ShipId? RepairingShipId { get; set; }

        [JsonPropertyName("api_complete_time")]
        public DateTimeOffset? CompletionTime { get; set; }

        public int api_item1 { get; set; }
        public int api_item3 { get; set; }
        public Materials Consumption => new Materials()
        {
            Fuel = api_item1,
            Steel = api_item3,
        };
    }
}
