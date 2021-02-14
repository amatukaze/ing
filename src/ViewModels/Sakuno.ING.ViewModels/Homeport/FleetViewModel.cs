using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class FleetViewModel : ReactiveObject, IHomeportTabViewModel
    {
        public FleetId Id { get; }

        public IReadOnlyCollection<ShipViewModel> Ships { get; }

        private readonly ObservableAsPropertyHelper<int> _totalLevel;
        public int TotalLevel => _totalLevel.Value;

        private readonly ObservableAsPropertyHelper<int> _speed;
        public ShipSpeed Speed => (ShipSpeed)_speed.Value;

        private readonly ObservableAsPropertyHelper<bool> _isSpeedVisible;
        public bool IsSpeedVisible => _isSpeedVisible.Value;

        private readonly ObservableAsPropertyHelper<int> _totalFirepower;
        public int TotalFirepower => _totalFirepower.Value;

        private readonly ObservableAsPropertyHelper<int> _totalAntiAir;
        public int TotalAntiAir => _totalAntiAir.Value;

        private readonly ObservableAsPropertyHelper<int> _totalAntiSubmarine;
        public int TotalAntiSubmarine => _totalAntiSubmarine.Value;

        private readonly ObservableAsPropertyHelper<int> _totalLoS;
        public int TotalLoS => _totalLoS.Value;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public FleetViewModel(PlayerFleet fleet)
        {
            Id = fleet.Id;

            var ships = fleet.Ships.ToObservableChangeSet();

            Ships = ships.Transform(r => new ShipViewModel(r)).Bind();

            _totalLevel = ships.AutoRefresh(r => r.Leveling).QueryWhenChanged(ships => ships.Sum(r => r.Leveling.Level))
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(TotalLevel));

            var speed = ships.AutoRefresh(r => r.Speed).QueryWhenChanged(ships => ships.Count != 0 ? ships.Min(r => (int)r.Speed) : 0);

            _speed = speed.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Speed));
            _isSpeedVisible = speed.Select(r => r > 0).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(IsSpeedVisible));

            _totalFirepower = ships.AutoRefresh(r => r.Firepower).QueryWhenChanged(ships => ships.Sum(r => r.Firepower.Displaying))
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(TotalFirepower));
            _totalAntiAir = ships.AutoRefresh(r => r.AntiAir).QueryWhenChanged(ships => ships.Sum(r => r.AntiAir.Displaying))
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(TotalAntiAir));
            _totalAntiSubmarine = ships.AutoRefresh(r => r.AntiSubmarine).QueryWhenChanged(ships => ships.Sum(r => r.AntiSubmarine.Displaying))
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(TotalAntiSubmarine));
            _totalLoS = ships.AutoRefresh(r => r.LineOfSight).QueryWhenChanged(ships => ships.Sum(r => r.LineOfSight.Displaying))
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(TotalLoS));
        }
    }
}
