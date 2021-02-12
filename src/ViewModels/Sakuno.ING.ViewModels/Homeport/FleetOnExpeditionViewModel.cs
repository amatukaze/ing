using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class FleetOnExpeditionViewModel : ReactiveObject, IFleetViewModel
    {
        public FleetId Id { get; }

        private readonly ObservableAsPropertyHelper<ExpeditionInfo> _expedition;
        public ExpeditionInfo Expedition => _expedition.Value;

        private readonly ObservableAsPropertyHelper<DateTimeOffset?> _completionTime;
        public DateTimeOffset? CompletionTime => _completionTime.Value;

        public FleetOnExpeditionViewModel(PlayerFleet fleet)
        {
            Id = fleet.Id;

            _expedition = fleet.WhenAnyValue(r => r.Expedition)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Expedition));
            _completionTime = fleet.WhenAnyValue(r => r.ExpeditionCompletionTime)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(CompletionTime));
        }
    }
}
