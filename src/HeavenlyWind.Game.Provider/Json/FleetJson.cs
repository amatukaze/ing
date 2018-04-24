using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    internal class FleetJson : IRawFleet
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_mission")]
        public FleetExpeditionState ExpeditionState { get; set; }
        [JsonProperty("api_ship")]
        public IReadOnlyList<int> ShipIds { get; set; }
    }
}
