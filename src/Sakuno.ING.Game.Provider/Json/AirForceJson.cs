using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class AirForceJson : IRawAirForceGroup
    {
        int IIdentifiable<int>.Id => (MapAreaId << 16) + GroupId;

        [JsonProperty("api_rid")]
        public int GroupId { get; }
        [JsonProperty("api_area_id")]
        public int MapAreaId { get; }
        [JsonProperty("api_name")]
        public string Name { get; }
        [JsonProperty("api_distance")]
        public int Distance { get; }
        [JsonProperty("api_action_kind")]
        public AirForceAction Action { get; }
        public AirForceSquadronJson[] api_plane_info;
        IReadOnlyCollection<IRawAirForceSquadron> IRawAirForceGroup.Squadrons => api_plane_info;
    }

    internal class AirForceSquadronJson : IRawAirForceSquadron
    {
        [JsonProperty("api_squadron_id")]
        public int Id { get; }
        [JsonProperty("api_slotid")]
        public int EquipmentId { get; }

        public int api_count;
        public int api_max_count;
        public ClampedValue AircraftCount => (api_count, api_max_count);

        [JsonProperty("api_cond")]
        public SquadronMorale Morale { get; }
    }
}
