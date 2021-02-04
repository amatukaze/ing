using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public class RepairDocksViewModel : ReactiveObject
    {
        public IReadOnlyCollection<IDockViewModel> RepairDocks { get; }

        public RepairDocksViewModel(NavalBase navalBase)
        {
            RepairDocks = navalBase.RepairDocks.DefaultViewSource
                .AutoRefresh(r => r.State)
                .Transform(r => r.State != RepairDockState.Locked ? (IDockViewModel)new RepairDockViewModel(r) : new LockedDockViewModel(r.Id)).Bind();
        }
    }
}
