using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public struct RawAerialSide
    {
        public ClampedValue FightedPlanes { readonly get; set; }
        public ClampedValue ShootedPlanes { readonly get; set; }
        public EquipmentInfoId? TouchingPlane { readonly get; set; }
        public IReadOnlyCollection<int> PlanesFrom { readonly get; set; }
    }
}
