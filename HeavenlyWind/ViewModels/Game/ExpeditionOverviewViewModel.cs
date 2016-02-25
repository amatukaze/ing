using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class ExpeditionOverviewViewModel : ModelBase
    {
        public IList<ExpeditionGroupByMapArea> MapAreas { get; private set; }

        IDisposable r_MasterInfoSubscription;

        internal ExpeditionOverviewViewModel()
        {
            r_MasterInfoSubscription = SessionService.Instance.Subscribe("api_port/port", delegate
            {
                MapAreas = KanColleGame.Current.MasterInfo.Expeditions.Values.Where(r => ExpeditionService.Instance.ContainsInfo(r.ID)).GroupBy(r => r.MapArea).Select(r => new ExpeditionGroupByMapArea(r)).ToList();
                OnPropertyChanged(nameof(MapAreas));

                r_MasterInfoSubscription.Dispose();
                r_MasterInfoSubscription = null;
            });
        }
    }
}
