using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class ShipViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<ShipInfo> _info;
        public ShipInfo Info => _info.Value;

        public IReadOnlyCollection<SlotViewModel> Slots { get; }

        public ShipViewModel(PlayerShip ship)
        {
            _info = ship.WhenAnyValue(r=>r.Info).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Info));

            Slots = ship.Slots.AsObservableChangeSet().Transform(r => new SlotViewModel(r)).Bind();
        }
    }
}
