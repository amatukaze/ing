using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Battle
{
    public interface IRawMapRouting
    {
        MapId MapId { get; }
        int RouteId { get; }
        MapEventKind EventKind { get; }
        BattleKind BattleKind { get; }
        bool CanMoveAdvance { get; }
        bool Reconnaissance { get; }
        string Message { get; }
        bool CanSelectRoute { get; }
        IReadOnlyCollection<UseItemRecord> ItemAcquired { get; }
        IReadOnlyCollection<UseItemRecord> ItemLost { get; }
        int? RankingPointAcquired { get; }
        IRawBattle LandBaseDefence { get; }
        bool MapPartUnlocked { get; }
    }
}
