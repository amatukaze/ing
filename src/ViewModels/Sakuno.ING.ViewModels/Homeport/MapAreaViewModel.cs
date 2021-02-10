using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class MapAreaViewModel : ReactiveObject, IHomeportTabViewModel
    {
        public MapAreaInfo Model { get; }

        public IReadOnlyCollection<AirForceGroupViewModel> Groups { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public MapAreaViewModel(MapAreaInfo mapAreaInfo, IObservable<IChangeSet<AirForceGroup, (MapAreaId MapArea, AirForceGroupId Group)>> groups)
        {
            Model = mapAreaInfo;

            Groups = groups.Transform(r => new AirForceGroupViewModel(r)).Bind();
        }
    }
}
