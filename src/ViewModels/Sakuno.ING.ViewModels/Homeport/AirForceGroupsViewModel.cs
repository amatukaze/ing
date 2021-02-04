using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class AirForceGroupsViewModel : ReactiveObject
    {
        public IReadOnlyCollection<AirForceGroupsOfAreaViewModel> Areas { get; }

        public AirForceGroupsViewModel(NavalBase navalBase)
        {
            Areas = navalBase.AirForceGroups.DefaultViewSource.GroupOnProperty(r => r.Area)
                .Transform(r => new AirForceGroupsOfAreaViewModel(r.Key, r.Cache.Connect())).Bind();
        }
    }
}
