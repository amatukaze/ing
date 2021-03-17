using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class ShipsDismantlingJson : IMaterialUpdate
    {
        public Materials api_material { get; set; }

        public void Apply(Materials materials)
        {
            materials.Fuel = api_material.Fuel;
            materials.Bullet = api_material.Bullet;
            materials.Steel = api_material.Steel;
            materials.Bauxite = api_material.Bauxite;
        }
    }
#nullable enable
}
