using Sakuno.ING.Game.Models.MasterData;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
#nullable disable
    public sealed class RawAirForceGroup : IIdentifiable<(MapAreaId MapArea, AirForceGroupId Group)>
    {
        [JsonPropertyName("api_rid")]
        public AirForceGroupId GroupId { get; set; }
        [JsonPropertyName("api_area_id")]
        public MapAreaId MapAreaId { get; set; }
        public (MapAreaId, AirForceGroupId) Id => (MapAreaId, GroupId);

        [JsonPropertyName("api_name")]
        public string Name { get; set; }

        public CombatRadius api_distance { get; set; }
        public int BaseCombatRadius => api_distance.api_base;
        public int BonusCombatRadius => api_distance.api_bonus;

        [JsonPropertyName("api_action_kind")]
        public AirForceAction Action { get; set; }

        [JsonPropertyName("api_plane_info")]
        public RawAirForceSquadron[] Squadrons { get; set; }

        public sealed class CombatRadius
        {
            public int api_base { get; set; }
            public int api_bonus { get; set; }
        }
    }
#nullable enable
}
