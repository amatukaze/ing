using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public interface IMaterialsUpdate
    {
        void Apply(ref Materials materials);

        MaterialsChangeReason Reason { get; }
    }

    public enum MaterialsChangeReason
    {
        Unknown,
        EquipmentCreate,
        EquipmentDismantle,
        ShipCreate,
        ShipDismantle,
        ShipSupply,
        AirForcePlaneSet,
        AirForceSupply,
        QuestComplete,
    }
}
