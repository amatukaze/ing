using System;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class BuildingDockJson : IRawBuildingDock
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }

        public long api_complete_time;
        public DateTimeOffset CompletionTime => DateTimeOffset.FromUnixTimeMilliseconds(api_complete_time);

        [JsonProperty("api_state")]
        public BuildingDockState State { get; set; }

        public int api_item1, api_item2, api_item3, api_item4, api_item5;
        public Materials Consumption => new Materials
        {
            Fuel = api_item1,
            Bullet = api_item2,
            Steel = api_item3,
            Bauxite = api_item4,
            Development = api_item5
        };

        [JsonProperty("api_created_ship_id")]
        public int BuiltShipId { get; set; }

        public bool IsLSC => api_item1 >= 1000;
    }
}
