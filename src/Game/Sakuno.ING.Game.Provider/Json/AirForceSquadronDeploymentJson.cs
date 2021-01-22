using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class AirForceSquadronDeploymentJson : IMaterialUpdate
    {
        public RawAirForceGroup.CombatRadius api_distance { get; set; }
        public RawAirForceSquadron[] api_plane_info { get; set; }
        public int? api_after_bauxite { get; set; }

        public void Apply(Materials materials)
        {
            if (api_after_bauxite is int bauxite)
                materials.Bauxite = bauxite;
        }
    }
#nullable enable
}
