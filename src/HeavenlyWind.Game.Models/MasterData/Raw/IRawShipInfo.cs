using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData.Raw
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

        ShipMordenizationStatus HP { get; }
        ShipMordenizationStatus Armor { get; }
        ShipMordenizationStatus Firepower { get; }
        ShipMordenizationStatus Torpedo { get; }
        ShipMordenizationStatus AntiAir { get; }
        ShipMordenizationStatus AntiSubmarine { get; }
        ShipMordenizationStatus Luck { get; }

        ShipSpeed Speed { get; }
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
