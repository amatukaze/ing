using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class RepairDockViewModel : ReactiveObject, IViewContractObservable
    {
        public RepairDockId Id { get; }

        public IObservable<string?> ViewContractObservable { get; }

        private readonly ObservableAsPropertyHelper<PlayerShip> _repairingShip;
        public PlayerShip RepairingShip => _repairingShip.Value;

        private readonly ObservableAsPropertyHelper<DateTimeOffset?> _completionTime;
        public DateTimeOffset? CompletionTime => _completionTime.Value;

        public RepairDockViewModel(RepairDock repairDock)
        {
            Id = repairDock.Id;

            ViewContractObservable = repairDock.WhenAnyValue(r => r.State).Select(r => r.ToString());

            _repairingShip = repairDock.WhenAnyValue(r => r.RepairingShip)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(RepairingShip));
            _completionTime = repairDock.WhenAnyValue(r => r.CompletionTime)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(CompletionTime));
        }
    }
}
