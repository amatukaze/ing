using System;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.MasterData
{
    public readonly struct EquipmentTypeId : IEquatable<EquipmentTypeId>, IComparable<EquipmentTypeId>
    {
        private readonly int value;
        public EquipmentTypeId(int value) => this.value = value;

        public int CompareTo(EquipmentTypeId other) => value - other.value;
        public bool Equals(EquipmentTypeId other) => value == other.value;

        public static implicit operator int(EquipmentTypeId id) => id.value;
        public static explicit operator EquipmentTypeId(long value) => new EquipmentTypeId((int)value);

        public static implicit operator EquipmentTypeId(KnownEquipmentType known) => new EquipmentTypeId((int)known);
        public static explicit operator KnownEquipmentType(EquipmentTypeId id) => (KnownEquipmentType)id.value;

        public override string ToString() => value.ToString();
    }

    public interface IRawEquipmentTypeInfo : IIdentifiable<EquipmentTypeId>
    {
        string Name { get; }

        bool AvailableInExtraSlot { get; }
    }
}
