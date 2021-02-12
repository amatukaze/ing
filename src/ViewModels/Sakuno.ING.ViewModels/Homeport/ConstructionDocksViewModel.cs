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
        public IReadOnlyCollection<ConstructionDockViewModel> ConstructionDocks { get; }

        public ConstructionDocksViewModel(NavalBase navalBase)
        {
            ConstructionDocks = navalBase.ConstructionDocks.DefaultViewSource.Transform(r => new ConstructionDockViewModel(r)).Bind();
        }
    }
}
