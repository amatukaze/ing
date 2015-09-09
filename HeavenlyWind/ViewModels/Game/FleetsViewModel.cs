using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetsViewModel : ModelBase
    {
        IReadOnlyList<FleetViewModel> r_Fleets;
        public IReadOnlyList<FleetViewModel> Fleets
        {
            get { return r_Fleets; }
            private set
            {
                if (r_Fleets != value)
                {
                    r_Fleets = value;
                    OnPropertyChanged(nameof(Fleets));
                }
            }
        }
        FleetViewModel r_SelectedFleet;
        public FleetViewModel SelectedFleet
        {
            get { return r_SelectedFleet; }
            private set
            {
                if (r_SelectedFleet != value)
                {
                    r_SelectedFleet = value;
                    OnPropertyChanged(nameof(SelectedFleet));
                }
            }
        }

        internal FleetsViewModel()
        {
            KanColleGame.Current.Port.Fleets.FleetsUpdated += UpdateFleets;
        }

        void UpdateFleets(IEnumerable<Fleet> rpFleets)
        {
            Fleets = KanColleGame.Current.Port.Fleets.Table.Values.Select(r => new FleetViewModel(r)).ToList();
            SelectedFleet = Fleets.FirstOrDefault();
        }
    }
}
