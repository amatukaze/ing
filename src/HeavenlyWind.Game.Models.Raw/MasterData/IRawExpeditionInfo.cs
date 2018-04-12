using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawExpeditionInfo
    {
        int Id { get; }
        string DisplayId { get; }
        int MapAreaId { get; }

        string Name { get; }
        string Description { get; }
        TimeSpan Duration { get; }

        int RequiredShipCount { get; }
        int Difficulty { get; }
        double FuelConsumption { get; }
        double BulletConsumption { get; }

        IReadOnlyList<ItemRecord> RewardItems { get; }

        bool CanRecall { get; }
    }
}
