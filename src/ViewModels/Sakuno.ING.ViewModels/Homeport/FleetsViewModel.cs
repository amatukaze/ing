using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class FleetsViewModel : ReactiveObject
    {
        public IReadOnlyCollection<FleetViewModel> Fleets { get; }

        public FleetsViewModel(NavalBase navalBase)
        {
            Fleets = navalBase.Fleets.DefaultViewSource.Transform(r => new FleetViewModel(r)).Bind();
        }
    }
}
