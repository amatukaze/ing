using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    public readonly struct ShipInfoId : IEquatable<ShipInfoId>, IComparable<ShipInfoId>
    {
        private readonly int value;
        public ShipInfoId(int value) => this.value = value;

        public int CompareTo(ShipInfoId other) => value - other.value;
        public bool Equals(ShipInfoId other) => value == other.value;

        public static implicit operator int(ShipInfoId id) => id.value;
        public static explicit operator ShipInfoId(long value) => new ShipInfoId((int)value);

        public override string ToString() => value.ToString();
    }

    public interface IRawShipInfo : IIdentifiable<ShipInfoId>
    {
        int SortNo { get; }
        string Name { get; }
        string Phonetic { get; }
        string Introduction { get; }

        bool IsAbyssal { get; }
        string AbyssalClass { get; }

        ShipTypeId TypeId { get; }
        int ClassId { get; }

        int UpgradeLevel { get; }
        ShipInfoId UpgradeTo { get; }
        Materials UpgradeConsumption { get; }
        IReadOnlyCollection<ItemRecord> UpgradeSpecialConsumption { get; }

        /// <summary>
        /// 耐久
        /// </summary>
        ShipMordenizationStatus HP { get; }
        /// <summary>
        /// 装甲
        /// </summary>
        ShipMordenizationStatus Armor { get; }
        /// <summary>
        /// 火力
        /// </summary>
        ShipMordenizationStatus Firepower { get; }
        /// <summary>
        /// 雷装
        /// </summary>
        ShipMordenizationStatus Torpedo { get; }
        /// <summary>
        /// 対空
        /// </summary>
        ShipMordenizationStatus AntiAir { get; }
        /// <summary>
        /// 運
        /// </summary>
        ShipMordenizationStatus Luck { get; }

        /// <summary>
        /// 速力
        /// </summary>
        ShipSpeed Speed { get; }
        /// <summary>
        /// 射程
        /// </summary>
        FireRange FireRange { get; }
        int SlotCount { get; }
        IReadOnlyList<int> Aircraft { get; }
        int Rarity { get; }

        TimeSpan ConstructionTime { get; }
        Materials DismantleAcquirement { get; }

        IReadOnlyList<int> PowerupWorth { get; }
        int FuelConsumption { get; }
        int BulletConsumption { get; }
    }
}
