using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class FleetJson : IRawFleet
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }

        public long[] api_mission;
        public FleetExpeditionState ExpeditionState => (FleetExpeditionState)api_mission.ElementAtOrDefault(0);
        public int ExpeditionId => (int)api_mission.ElementAtOrDefault(1);
        public DateTimeOffset ExpeditionCompletionTime => DateTimeOffset.FromUnixTimeMilliseconds(api_mission.ElementAtOrDefault(2));

        [JsonProperty("api_ship")]
        public IReadOnlyList<int> ShipIds { get; set; }
    }
}
