using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class BattleResult
    {
        public BattleResult(MasterDataRoot masterData, RawBattleResult raw, Side side)
        {
            Rank = raw.Rank;
            AdmiralExperience = raw.AdmiralExperience;
            BaseExperience = raw.BaseExperience;
            MapCleared = raw.MapCleared;
            EnemyFleetName = raw.EnemyFleetName;
            AirReconnaissanceSuccessed = raw.AirReconnaissanceSuccessed;
            UseItemAcquired = masterData.UseItems[raw.UseItemAcquired];
            ShipDropped = masterData.ShipInfos[raw.ShipDropped];
            RankingPointAcquired = raw.RankingPointAcquired;
            TransportationPoint = raw.TransportationPoint;
            EscapableShips = raw.EscapableShipIndices.Select(side.FindShip).ToArray();
        }

        public BattleRank Rank { get; }
        public int AdmiralExperience { get; }
        public int BaseExperience { get; }
        public bool MapCleared { get; }
        public string EnemyFleetName { get; }
        public bool? AirReconnaissanceSuccessed { get; }
        public UseItemInfo UseItemAcquired { get; }
        public ShipInfo ShipDropped { get; }
        public int? RankingPointAcquired { get; }
        public int? TransportationPoint { get; }
        public IReadOnlyList<BattleParticipant> EscapableShips { get; }
    }
}
