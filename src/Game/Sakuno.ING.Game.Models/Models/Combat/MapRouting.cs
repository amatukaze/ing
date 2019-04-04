using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public class MapRouting
    {
        public MapRouting(NavalBase navalBase, RawMapRouting m)
        {
            Map = navalBase.Maps[m.MapId];
            RouteId = m.RouteId;
            EventKind = m.EventKind;
            if (EventKind == MapEventKind.Battle)
                BattleKind = m.BattleKind;
            CanMoveAdvance = m.CanMoveAdvance;
            Reconnaissance = m.Reconnaissance;
            Message = m.Message;
            SelectableRoutes = m.SelectableRoutes;
            RankingPointAcquired = m.RankingPointAcquired;
            UseItemChanges = m.ItemAcquired.Select(x => new UseItemChange(navalBase.MasterData.UseItems[x.ItemId], x.Count))
                .Concat(m.ItemLost.Select(x => new UseItemChange(navalBase.MasterData.UseItems[x.ItemId], -x.Count)))
                .ToArray();
        }

        public Map Map { get; }
        public int RouteId { get; }
        public MapEventKind EventKind { get; }
        public BattleKind BattleKind { get; }
        public bool CanMoveAdvance { get; }
        public bool Reconnaissance { get; }
        public string Message { get; }
        public IReadOnlyCollection<int> SelectableRoutes { get; }
        public bool CanSelectRoute => SelectableRoutes != null;
        public int? RankingPointAcquired { get; }
        public IReadOnlyCollection<UseItemChange> UseItemChanges { get; }
    }
}
