using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public interface IRawBattleResult
    {
        BattleRank Rank { get; }
        int AdmiralExperience { get; }
        int BaseExperience { get; }
        bool MapCleared { get; }
        string EnemyFleetName { get; }
        bool? AirReconnaissanceSuccessed { get; }
        UseItemId? UseItemAcquired { get; }
        ShipInfoId? ShipDropped { get; }
        int? RankingPointAcquired { get; }
        int? TransportationPoint { get; }
        IReadOnlyCollection<IRawReward> Rewards { get; }
        IReadOnlyCollection<int> EscapableShipIndices { get; }
    }
}
