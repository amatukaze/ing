using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class FleetsViewModel : ReactiveObject
    {
        public IReadOnlyCollection<FleetViewModel> Fleets { get; }

        private FleetViewModel? _selectedFleet;
        public FleetViewModel? SelectedFleet
        {
            get => _selectedFleet;
            internal set => this.RaiseAndSetIfChanged(ref _selectedFleet, value);
        }

        public FleetsViewModel(NavalBase navalBase)
        {
            var fleets = navalBase.Fleets.DefaultViewSource.Transform(r => new FleetViewModel(r));

            Fleets = fleets.Bind();

            fleets.Where(r => r.Adds > 0).Take(1)
                .ObserveOn(RxApp.MainThreadScheduler).Subscribe(r => SelectedFleet = Fleets.First());
        }
    }
}
