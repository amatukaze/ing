using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public class ConstructionDocksViewModel : ReactiveObject
    {
        public IReadOnlyCollection<IDockViewModel> ConstructionDocks { get; }

        public ConstructionDocksViewModel(NavalBase navalBase)
        {
            ConstructionDocks = navalBase.ConstructionDocks.DefaultViewSource
                .AutoRefresh(r => r.State)
                .Transform(r => r.State != ConstructionDockState.Locked ? (IDockViewModel)new ConstructionDockViewModel(r) : new LockedDockViewModel(r.Id)).Bind();
        }
    }
}
