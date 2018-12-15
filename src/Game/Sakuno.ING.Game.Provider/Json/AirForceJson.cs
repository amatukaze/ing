using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal class AirForceJson : IRawAirForceGroup
    {
        (MapAreaId, AirForceGroupId) IIdentifiable<(MapAreaId MapArea, AirForceGroupId GroupId)>.Id => (MapAreaId, GroupId);

        [JsonProperty("api_rid")]
        public AirForceGroupId GroupId { get; set; }
        [JsonProperty("api_area_id")]
        public MapAreaId MapAreaId { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }


        public class Distance
        {
            public int api_base;
            public int api_bonus;
        }
        public Distance api_distance;
        public int DistanceBase => api_distance.api_base;
        public int DistanceBonus => api_distance.api_bonus;

        [JsonProperty("api_action_kind")]
        public AirForceAction Action { get; set; }
        public AirForceSquadronJson[] api_plane_info;
        IReadOnlyCollection<IRawAirForceSquadron> IRawAirForceGroup.Squadrons => api_plane_info;
    }

    internal class AirForceSquadronJson : IRawAirForceSquadron
    {
        [JsonProperty("api_squadron_id")]
        public int Id { get; set; }
        [JsonProperty("api_slotid")]
        public EquipmentId? EquipmentId { get; set; }

        public int api_count;
        public int api_max_count;
        public ClampedValue AircraftCount => (api_count, api_max_count);

        [JsonProperty("api_cond")]
        public SquadronMorale Morale { get; set; }
    }
}
