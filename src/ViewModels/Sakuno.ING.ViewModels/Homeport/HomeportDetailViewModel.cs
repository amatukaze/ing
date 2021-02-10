using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class HomeportDetailViewModel : ReactiveObject
    {
        public IReadOnlyCollection<FleetViewModel> Fleets { get; }
        public IReadOnlyCollection<MapAreaViewModel> Areas { get; }

        private readonly ObservableAsPropertyHelper<IHomeportTabViewModel?> _selectedTab;
        public IHomeportTabViewModel? SelectedTab => _selectedTab.Value;

        public HomeportDetailViewModel(NavalBase navalBase)
        {
            var fleets = navalBase.Fleets.DefaultViewSource.Transform(r => new FleetViewModel(r)).Bind();
            var areas = navalBase.AirForceGroups.DefaultViewSource.GroupOnProperty(r => r.Area)
                .Transform(r => new MapAreaViewModel(r.Key, r.Cache.Connect())).Bind();

            Fleets = fleets;
            Areas = areas;

            var fleetsObservable = fleets.ToObservableChangeSet();
            var fleetsCountChanged = fleetsObservable.Count().StartWith(0).Buffer(2, 1);
            var selectedFleet = fleetsObservable
                .WhenPropertyChanged(r => r.IsSelected).Where(r => r.Value).Select(r => (IHomeportTabViewModel)r.Sender);
            var selectedArea = areas.ToObservableChangeSet()
                .WhenPropertyChanged(r => r.IsSelected).Where(r => r.Value).Select(r => (IHomeportTabViewModel)r.Sender);

            fleetsCountChanged.Where(r => r[0] is 0).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ => Fleets.First().IsSelected = true);

            _selectedTab = Observable.Merge(
                fleetsCountChanged.Where(r => r[1] is 0).Select(_ => (IHomeportTabViewModel?)null),
                selectedFleet,
                selectedArea
            ).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(SelectedTab));
        }
    }
}
