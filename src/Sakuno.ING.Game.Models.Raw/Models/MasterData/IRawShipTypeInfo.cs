using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.MasterData
{
    [Identifier(typeof(int))]
    public readonly struct ShipTypeId : IEquatable<ShipTypeId>, IComparable<ShipTypeId>
    {
        private readonly int value;
        public ShipTypeId(int value) => this.value = value;

        public int CompareTo(ShipTypeId other) => value - other.value;
        public bool Equals(ShipTypeId other) => value == other.value;

        public static implicit operator int(ShipTypeId id) => id.value;
        public static explicit operator ShipTypeId(int value) => new ShipTypeId(value);

        public static implicit operator ShipTypeId(KnownShipType known) => new ShipTypeId((int)known);
        public static explicit operator KnownShipType(ShipTypeId id) => (KnownShipType)id.value;

        public override string ToString() => value.ToString();
    }

    public interface IRawShipTypeInfo : IIdentifiable<ShipTypeId>
    {
        int SortNo { get; }
        string Name { get; }

        int RepairTimeRatio { get; }
        int BuildOutlineId { get; }
        IReadOnlyCollection<EquipmentTypeId> AvailableEquipmentTypes { get; }
    }
}
