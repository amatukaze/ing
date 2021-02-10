using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class FleetOverviewViewModel : ReactiveObject
    {
        public IReadOnlyCollection<IFleetViewModel> Fleets { get; }

        public FleetOverviewViewModel(NavalBase navalBase)
        {
            Fleets = navalBase.Fleets.DefaultViewSource
                .AutoRefresh(r => r.ExpeditionState)
                .Transform(r => r.ExpeditionState == FleetExpeditionState.None ? (IFleetViewModel)new IdleFleetViewModel(r) : new FleetOnExpeditionViewModel(r)).Bind();
        }
    }
}
