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
        IReadOnlyCollection<ItemRecord> ItemAcquired { get; }
        IReadOnlyCollection<ItemRecord> ItemLost { get; }
        int? RankingPointAcquired { get; }
    }
}
