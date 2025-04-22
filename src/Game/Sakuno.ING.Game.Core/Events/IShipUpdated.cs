using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IShipUpdated : IIdentifiable<ShipId>
{
    ShipInfoId MasterId { get; }

    int Level { get; }
    int TotalExperience { get; }
    int ExperienceToNextLevel { get; }

    ShipHP HP { get; }

    ShipSpeed Speed { get; }
    FireRange FireRange { get; }

    int Fuel { get; }
    int Bullet { get; }

    IReadOnlyList<SlotItemId> SlotItems { get; }
    IReadOnlyList<int> PlaneCount { get; }
    SlotItemId ExtraSlot { get; }

    int Morale { get; }

    int[] ImprovedStatuses { get; }
    int Firepower { get; }
    int Torpedo { get; }
    int AntiAir { get; }
    int Armor { get; }
    int Evasion { get; }
    int AntiSubmarine { get; }
    int LineOfSight { get; }
    int Luck { get; }

    bool IsLocked { get; }

    int? SortieLockingTagId { get; }
}
