using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Shipyard
{
    internal class ShipDismantleJson : IMaterialsUpdate
    {
        public int[] api_material;

        MaterialsChangeReason IMaterialsUpdate.Reason => MaterialsChangeReason.ShipDismantle;

        void IMaterialsUpdate.Apply(ref Materials materials)
        {
            if (api_material is null) return;
            if (api_material.Length >= 1) materials.Fuel = api_material[0];
            if (api_material.Length >= 2) materials.Bullet = api_material[1];
            if (api_material.Length >= 3) materials.Steel = api_material[2];
            if (api_material.Length >= 4) materials.Bauxite = api_material[3];
            if (api_material.Length >= 5) materials.InstantBuild = api_material[4];
            if (api_material.Length >= 6) materials.InstantRepair = api_material[5];
            if (api_material.Length >= 7) materials.Development = api_material[6];
            if (api_material.Length >= 8) materials.Improvement = api_material[7];
        }
    }
}
