using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawAerialSide
    {
        ClampedValue FightedPlanes { get; }
        ClampedValue ShootedPlanes { get; }
        EquipmentInfoId? TouchingPlane { get; }
        IRawAntiAirFire AntiAirFire { get; }
    }
}
