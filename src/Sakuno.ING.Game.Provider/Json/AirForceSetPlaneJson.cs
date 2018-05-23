using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class AirForceSetPlaneJson : IMaterialsUpdate
    {
        public int api_distance;
        public AirForceSquadronJson[] api_plane_info;
        public int? api_after_bauxite;

        MaterialsChangeReason IMaterialsUpdate.Reason => MaterialsChangeReason.AirForcePlaneSet;

        void IMaterialsUpdate.Apply(ref Materials materials)
        {
            if (api_after_bauxite is int value)
                materials.Bauxite = value;
        }
    }
}
