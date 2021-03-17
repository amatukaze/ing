using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class SlotItemsScrappingJson : IMaterialUpdate
    {
        public Materials api_get_material { get; set; }

        public void Apply(Materials materials)
        {
            materials.Fuel += api_get_material.Fuel;
            materials.Bullet += api_get_material.Bullet;
            materials.Steel += api_get_material.Steel;
            materials.Bauxite += api_get_material.Bauxite;
        }
    }
#nullable enable
}
