using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawShipInfo
    {
        int Id { get; }
        int SortNo { get; }
        string Name { get; }
        string Phonetic { get; }
        string Introduction { get; }

        bool IsAbyssal { get; }
        string AbyssalClass { get; }

        int TypeId { get; }
        int ClassId { get; }

        int UpgradeLevel { get; }
        int UpgradeTo { get; }
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

        TimeSpan ConstructionTime { get; }
        Materials DismantleAcquirement { get; }

        IReadOnlyList<int> PowerupWorth { get; }
        int FuelConsumption { get; }
        int BulletConsumption { get; }
    }
}
