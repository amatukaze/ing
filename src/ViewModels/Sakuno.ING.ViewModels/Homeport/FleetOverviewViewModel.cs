using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Shell;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class FleetOverviewViewModel : ReactiveObject, IViewContractObservable
    {
        public FleetId Id { get; }

        public IObservable<string?> ViewContractObservable { get; }

        private readonly ObservableAsPropertyHelper<ExpeditionInfo> _expedition;
        public ExpeditionInfo Expedition => _expedition.Value;

        private readonly ObservableAsPropertyHelper<DateTimeOffset?> _completionTime;
        public DateTimeOffset? CompletionTime => _completionTime.Value;

        public FleetOverviewViewModel(PlayerFleet fleet)
        {
            Id = fleet.Id;

            ViewContractObservable = fleet.WhenAnyValue(r => r.ExpeditionState).Select(r => r switch
            {
                FleetExpeditionState.None => null,
                _ => "OnExpedition",
            });

            _expedition = fleet.WhenAnyValue(r => r.Expedition)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Expedition));
            _completionTime = fleet.WhenAnyValue(r => r.ExpeditionCompletionTime)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(CompletionTime));
        }
    }
}
