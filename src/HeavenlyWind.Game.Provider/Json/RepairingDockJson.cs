using System;
using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    internal class RepairingDockJson : IRawRepairingDock
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_state")]
        public RepairingDockState State { get; set; }
        [JsonProperty("api_ship_id")]
        public int RepairingShipId { get; set; }
        public long api_complete_time;
        public DateTimeOffset CompletionTime => DateTimeOffset.FromUnixTimeMilliseconds(api_complete_time);
        public int api_item1;
        public int api_item3;
        public Materials Consumption => new Materials
        {
            Fuel = api_item1,
            Steel = api_item3
        };
    }
}
