using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class AirForceGroupsOfAreaViewModel : ReactiveObject
    {
        public MapAreaInfo Model { get; }

        public IReadOnlyCollection<AirForceGroupViewModel> Groups { get; }

        public AirForceGroupsOfAreaViewModel(MapAreaInfo mapAreaInfo, IObservable<IChangeSet<AirForceGroup, (MapAreaId MapArea, AirForceGroupId Group)>> groups)
        {
            Model = mapAreaInfo;

            Groups = groups.Transform(r => new AirForceGroupViewModel(r)).Bind();
        }
    }
}
