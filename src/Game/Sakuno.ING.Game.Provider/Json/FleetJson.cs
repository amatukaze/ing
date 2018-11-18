using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal class FleetJson : IRawFleet
    {
        [JsonProperty("api_id")]
        public FleetId Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }

        public long[] api_mission;
        public FleetExpeditionState ExpeditionState => (FleetExpeditionState)api_mission.ElementAtOrDefault(0);
        public ExpeditionId? ExpeditionId
        {
            get
            {
                long id = api_mission.ElementAtOrDefault(1);
                if (id > 0)
                    return (ExpeditionId)id;
                else
                    return null;
            }
        }

        public DateTimeOffset? ExpeditionCompletionTime
        {
            get
            {
                long value = api_mission.ElementAtOrDefault(2);
                return value > 0 ?
                    DateTimeOffset.FromUnixTimeMilliseconds(value) :
                    (DateTimeOffset?)null;
            }
        }

        [JsonProperty("api_ship")]
        public IReadOnlyList<ShipId> ShipIds { get; set; }
    }
}
