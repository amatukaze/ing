using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class ExpeditionGroupByMapArea : ModelBase
    {
        public MapAreaInfo MapArea { get; }
        public IList<ExpeditionViewModel> Expeditions { get; }

        internal ExpeditionGroupByMapArea(IGrouping<MapAreaInfo, ExpeditionInfo> rpGroup)
        {
            MapArea = rpGroup.Key;
            Expeditions = rpGroup.Select(r => new ExpeditionViewModel(r)).ToList();
        }
    }
}
