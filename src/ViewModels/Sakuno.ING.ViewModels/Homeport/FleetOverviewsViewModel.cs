using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class FleetOverviewsViewModel : ReactiveObject
    {
        public IReadOnlyCollection<FleetOverviewViewModel> Fleets { get; }

        public FleetOverviewsViewModel(NavalBase navalBase)
        {
            Fleets = navalBase.Fleets.DefaultViewSource.Transform(r => new FleetOverviewViewModel(r)).Bind();
        }
    }
}
