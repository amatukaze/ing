using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(ExpeditionOverview))]
    public class ExpeditionOverviewViewModel : TabItemViewModel, IDisposable
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_ExpeditionOverview; }
            protected set { }
        }

        public IList<ExpeditionGroupByMapArea> MapAreas { get; private set; }

        IDisposable r_MasterInfoSubscription;
        IDisposable[] r_PCELs;

        internal ExpeditionOverviewViewModel()
        {
            if (KanColleGame.Current.Port.Ships.Count > 0)
                Initialize();
            else
                r_MasterInfoSubscription = SessionService.Instance.Subscribe("api_port/port", delegate
                {
                    Initialize();

                    r_MasterInfoSubscription.Dispose();
                    r_MasterInfoSubscription = null;
                });
        }

        void Initialize()
        {
            MapAreas = KanColleGame.Current.MasterInfo.Expeditions.Values.Where(r => ExpeditionService.Instance.ContainsInfo(r.ID)).GroupBy(r => r.MapArea).Select(r => new ExpeditionGroupByMapArea(r)).ToList();
            OnPropertyChanged(nameof(MapAreas));

            r_PCELs = KanColleGame.Current.Port.Fleets.Table.Values.Skip(1).Select(r =>
            {
                var rPCEL = new PropertyChangedEventListener(r);
                rPCEL.Add(nameof(r.Ships), (s, e) => Update(r));
                return rPCEL;
            }).ToArray();
        }

        void Update(Fleet rpFleet)
        {
            foreach (var rArea in MapAreas)
                foreach (var rExpedition in rArea.Expeditions)
                    rExpedition.Update(rpFleet);
        }

        public void Dispose()
        {
            if (r_PCELs != null)
                foreach (var rPCEL in r_PCELs)
                    rPCEL.Dispose();
            r_PCELs = null;
        }
    }
}
