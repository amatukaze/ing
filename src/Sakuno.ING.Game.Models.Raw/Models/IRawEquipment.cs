using System;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public readonly struct EquipmentId : IEquatable<EquipmentId>, IComparable<EquipmentId>
    {
        private readonly int value;
        public EquipmentId(int value) => this.value = value;

        public int CompareTo(EquipmentId other) => value - other.value;
        public bool Equals(EquipmentId other) => value == other.value;

        public static implicit operator int(EquipmentId id) => id.value;
        public static explicit operator EquipmentId(long value) => new EquipmentId((int)value);

        public override string ToString() => value.ToString();
    }

    public interface IRawEquipment : IIdentifiable<EquipmentId>
    {
        EquipmentInfoId EquipmentInfoId { get; }
        bool IsLocked { get; }
        int ImprovementLevel { get; }
        int AirProficiency { get; }
    }
}
