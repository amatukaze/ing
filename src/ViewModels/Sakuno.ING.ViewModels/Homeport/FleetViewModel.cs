using DynamicData.Aggregation;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class FleetViewModel : ReactiveObject
    {
        public PlayerFleet Model { get; }

        public IReadOnlyCollection<PlayerShip> Ships { get; }

        private ObservableAsPropertyHelper<int> _totalLevel;
        public int TotalLevel => _totalLevel.Value;

        private ObservableAsPropertyHelper<int> _speed;
        public ShipSpeed Speed => (ShipSpeed)_speed.Value;

        public FleetViewModel(PlayerFleet fleet)
        {
            Model = fleet;

            Ships = fleet.Ships.Bind();

            _totalLevel = fleet.Ships.Sum(r => r.Leveling.Level).ToProperty(this, nameof(TotalLevel), deferSubscription: true);
            _speed = fleet.Ships.Minimum(r => (int)r.Speed).ToProperty(this, nameof(ShipSpeed));
        }
    }
}
