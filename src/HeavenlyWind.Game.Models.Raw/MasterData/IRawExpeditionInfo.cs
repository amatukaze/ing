using System;

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

        int RewardItem1Id { get; }
        int RewardItem1Count { get; }
        int RewardItem2Id { get; }
        int RewardItem2Count { get; }

        bool CanRecall { get; }
    }
}
