using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class AirForceSupplyJson : IMaterialsUpdate
    {
        public int api_after_fuel;
        public int api_after_bauxite;
        public AirForceSquadronJson[] api_plane_info;

        MaterialsChangeReason IMaterialsUpdate.Reason => MaterialsChangeReason.AirForceSupply;

        void IMaterialsUpdate.Apply(ref Materials materials)
        {
            materials.Fuel = api_after_fuel;
            materials.Bauxite = api_after_bauxite;
        }
    }
}
