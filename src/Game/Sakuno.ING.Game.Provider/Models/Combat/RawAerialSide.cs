using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public struct RawAerialSide
    {
        public ClampedValue FightedPlanes { get; set; }
        public ClampedValue ShootedPlanes { get; set; }
        public EquipmentInfoId? TouchingPlane { get; set; }
    }
}
