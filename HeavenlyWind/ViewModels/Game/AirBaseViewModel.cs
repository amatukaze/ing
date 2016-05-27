using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(AirBaseOverview))]
    public class AirBaseViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return "A"; }
            protected set { throw new NotImplementedException(); }
        }

        IList<AirForceGroupViewModel> r_Groups;
        public IList<AirForceGroupViewModel> Groups
        {
            get { return r_Groups; }
            private set
            {
                if (r_Groups!= value)
                {
                    r_Groups = value;
                    OnPropertyChanged(nameof(Groups));
                }
            }
        }

        internal AirBaseViewModel()
        {
            var rAirBase = KanColleGame.Current.Port.AirBase;
            var rPCEL = new PropertyChangedEventListener(rAirBase);
            rPCEL.Add(nameof(rAirBase.Table), (s, e) => Groups = rAirBase.Table.Values.Select(r => new AirForceGroupViewModel(r)).ToList());
        }
    }
}
