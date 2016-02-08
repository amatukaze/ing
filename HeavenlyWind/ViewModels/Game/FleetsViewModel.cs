using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class FleetsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Fleets; }
            protected set { throw new NotImplementedException(); }
        }

        GameInformationViewModel r_Parent;

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

        internal FleetsViewModel(GameInformationViewModel rpParent)
        {
            r_Parent = rpParent;

            KanColleGame.Current.Port.Fleets.FleetsUpdated += UpdateFleets;

            SessionService.Instance.Subscribe("api_req_hensei/change", r => SelectedFleet = Fleets[int.Parse(r.Requests["api_id"]) - 1]);
            SessionService.Instance.Subscribe("api_req_hensei/preset_select", r => SelectedFleet = Fleets[int.Parse(r.Requests["api_deck_id"]) - 1]);

            SessionService.Instance.Subscribe("api_req_map/start", r => SelectedFleet = Fleets[int.Parse(r.Requests["api_deck_id"]) - 1]);
        }

        void UpdateFleets(IEnumerable<Fleet> rpFleets)
        {
            Fleets = KanColleGame.Current.Port.Fleets.Table.Values.Select(r => new FleetViewModel(r)).ToList();
            SelectedFleet = Fleets.FirstOrDefault();

            r_Parent.Overview.Fleets = Fleets;
        }
    }
}
