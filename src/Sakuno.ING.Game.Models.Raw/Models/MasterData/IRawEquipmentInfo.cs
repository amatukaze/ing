using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    public readonly struct EquipmentInfoId : IEquatable<EquipmentInfoId>, IComparable<EquipmentInfoId>
    {
        private readonly int value;
        public EquipmentInfoId(int value) => this.value = value;

        public int CompareTo(EquipmentInfoId other) => value - other.value;
        public bool Equals(EquipmentInfoId other) => value == other.value;

        public static implicit operator int(EquipmentInfoId id) => id.value;
        public static explicit operator EquipmentInfoId(long value) => new EquipmentInfoId((int)value);

        public override string ToString() => value.ToString();
    }

    public interface IRawEquipmentInfo : IIdentifiable<EquipmentInfoId>
    {
        string Name { get; }
        string Description { get; }

        EquipmentTypeId TypeId { get; }
        int IconId { get; }
        IReadOnlyCollection<ShipInfoId> ExtraSlotAcceptingShips { get; }

        /// <summary>
        /// 火力
        /// </summary>
        int Firepower { get; }
        /// <summary>
        /// 雷装
        /// </summary>
        int Torpedo { get; }
        /// <summary>
        /// 対空
        /// </summary>
        int AntiAir { get; }
        /// <summary>
        /// 装甲
        /// </summary>
        int Armor { get; }
        /// <summary>
        /// 爆装
        /// </summary>
        int DiveBomberAttack { get; }
        /// <summary>
        /// 対潜
        /// </summary>
        int AntiSubmarine { get; }
        /// <summary>
        /// 命中
        /// </summary>
        int Accuracy { get; }
        /// <summary>
        /// 回避
        /// </summary>
        int Evasion { get; }
        /// <summary>
        /// 対爆
        /// </summary>
        int AntiBomber { get; }
        /// <summary>
        /// 迎撃
        /// </summary>
        int Interception { get; }
        /// <summary>
        /// 索敵
        /// </summary>
        int LightOfSight { get; }
        /// <summary>
        /// 射程
        /// </summary>
        FireRange FireRange { get; }

        int FlightRadius { get; }
        Materials DeploymentConsumption { get; }
        Materials DismantleAcquirement { get; }

        int Rarity { get; }
    }
}
