using Sakuno.KanColle.Amatsukaze.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class OverviewViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Overview; }
            protected set { throw new NotImplementedException(); }
        }

        IReadOnlyList<FleetViewModel> r_Fleets;
        public IReadOnlyList<FleetViewModel> Fleets
        {
            get { return r_Fleets; }
            internal set
            {
                if (r_Fleets != value)
                {
                    r_Fleets = value;
                    OnPropertyChanged(nameof(Fleets));
                }
            }
        }

        IReadOnlyCollection<RepairDockViewModel> r_RepairDocks;
        public IReadOnlyCollection<RepairDockViewModel> RepairDocks
        {
            get { return r_RepairDocks; }
            private set
            {
                if (r_RepairDocks != value)
                {
                    r_RepairDocks = value;
                    OnPropertyChanged(nameof(RepairDocks));
                }
            }
        }
        IReadOnlyCollection<BuildingDockViewModel> r_BuildingDocks;
        public IReadOnlyCollection<BuildingDockViewModel> BuildingDocks
        {
            get { return r_BuildingDocks; }
            private set
            {
                if (r_BuildingDocks != value)
                {
                    r_BuildingDocks = value;
                    OnPropertyChanged(nameof(BuildingDocks));
                }
            }
        }

        internal OverviewViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rPort, nameof(rPort.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rPort.RepairDocks))
                .Subscribe(_ => RepairDocks = rPort.RepairDocks.Values.Select(r => new RepairDockViewModel(r)).ToList());
            rPropertyChangedSource.Where(r => r == nameof(rPort.BuildingDocks))
                .Subscribe(_ => BuildingDocks = rPort.BuildingDocks.Values.Select(r => new BuildingDockViewModel(r)).ToList());
        }
    }
}
