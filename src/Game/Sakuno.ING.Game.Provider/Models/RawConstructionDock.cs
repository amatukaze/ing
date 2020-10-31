using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawConstructionDock
    {
        [JsonPropertyName("api_id")]
        public ConstructionDockId Id { get; set; }

        [JsonPropertyName("api_complete_time")]
        public DateTimeOffset? CompletionTime { get; set; }

        [JsonPropertyName("api_state")]
        public ConstructionDockState State { get; set; }

        public int api_item1 { get; set; }
        public int api_item2 { get; set; }
        public int api_item3 { get; set; }
        public int api_item4 { get; set; }
        public int api_item5 { get; set; }
        public Materials Consumption => new Materials()
        {
            Fuel = api_item1,
            Bullet = api_item2,
            Steel = api_item3,
            Bauxite = api_item4,
            Development = api_item5
        };

        [JsonPropertyName("api_created_ship_id")]
        public ShipInfoId? BuiltShipId { get; set; }

        public bool IsLSC => api_item1 >= 1000;
    }

    public enum ConstructionDockState
    {
        Locked = -1,
        Idle = 0,
        Building = 2,
        Completed = 3,
    }
}
