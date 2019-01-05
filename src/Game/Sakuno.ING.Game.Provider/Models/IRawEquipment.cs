using System;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    [Identifier(typeof(int))]
    public readonly struct EquipmentId : IEquatable<EquipmentId>, IComparable<EquipmentId>
    {
        private readonly int value;
        public EquipmentId(int value) => this.value = value;

        public int CompareTo(EquipmentId other) => value - other.value;
        public bool Equals(EquipmentId other) => value == other.value;

        public static implicit operator int(EquipmentId id) => id.value;
        public static explicit operator EquipmentId(int value) => new EquipmentId(value);

        public static bool operator ==(EquipmentId left, EquipmentId right) => left.value == right.value;
        public static bool operator !=(EquipmentId left, EquipmentId right) => left.value != right.value;
        public override bool Equals(object obj) => (EquipmentId)obj == this;
        public override int GetHashCode() => value.GetHashCode();
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
