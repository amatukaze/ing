using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models
{
    public interface IRawShip : IIdentifiable
    {
        int ShipInfoId { get; }
        Leveling Leveling { get; }
        ClampedValue HP { get; }
        ShipSpeed Speed { get; }
        FireRange FireRange { get; }
        IReadOnlyList<int> EquipmentIds { get; }
        bool ExtraSlotOpened { get; }
        int ExtraSlotEquipId { get; }
        IReadOnlyList<int> SlotAircraft { get; }

        int CurrentFuel { get; }
        int CurrentBullet { get; }
        TimeSpan RepairingTime { get; }
        Materials RepairingCost { get; }
        int Morale { get; }

        ShipMordenizationStatus Firepower { get; }
        ShipMordenizationStatus Torpedo { get; }
        ShipMordenizationStatus AntiAir { get; }
        ShipMordenizationStatus Armor { get; }
        ShipMordenizationStatus Evasion { get; }
        ShipMordenizationStatus AntiSubmarine { get; }
        ShipMordenizationStatus LightOfSight { get; }
        ShipMordenizationStatus Luck { get; }

        bool IsLocked { get; }
        int? ShipLockingTag { get; }
    }
}
