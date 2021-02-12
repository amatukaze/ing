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
        public IReadOnlyCollection<RepairDockViewModel> RepairDocks { get; }

        public RepairDocksViewModel(NavalBase navalBase)
        {
            RepairDocks = navalBase.RepairDocks.DefaultViewSource.Transform(r => new RepairDockViewModel(r)).Bind();
        }
    }
}
