using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ShipId : IEquatable<ShipId>, IComparable<ShipId>
    {
        private readonly int value;
        public ShipId(int value) => this.value = value;

        public int CompareTo(ShipId other) => value - other.value;
        public bool Equals(ShipId other) => value == other.value;

        public static implicit operator int(ShipId id) => id.value;
        public static explicit operator ShipId(long value) => new ShipId((int)value);

        public override string ToString() => value.ToString();
    }

    public interface IRawShip : IIdentifiable<ShipId>
    {
        ShipInfoId ShipInfoId { get; }
        Leveling Leveling { get; }
        ClampedValue HP { get; }
        ShipSpeed Speed { get; }
        FireRange FireRange { get; }
        IReadOnlyList<EquipmentId> EquipmentIds { get; }
        bool ExtraSlotOpened { get; }
        EquipmentId ExtraSlotEquipId { get; }
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
