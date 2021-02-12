using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class ConstructionDockViewModel : ReactiveObject, IDockViewModel
    {
        public ConstructionDockId Id { get; }

        private readonly ObservableAsPropertyHelper<ShipInfo> _builtShip;
        public ShipInfo BuiltShip => _builtShip.Value;

        private readonly ObservableAsPropertyHelper<Materials> _consumption;
        public Materials Consumption => _consumption.Value;

        private readonly ObservableAsPropertyHelper<bool> _isCompleted;
        public bool IsCompleted => _isCompleted.Value;

        private readonly ObservableAsPropertyHelper<DateTimeOffset?> _completionTime;
        public DateTimeOffset? CompletionTime => _completionTime.Value;

        public ConstructionDockViewModel(ConstructionDock constructionDock)
        {
            Id = constructionDock.Id;

            _builtShip = constructionDock.WhenAnyValue(r => r.BuiltShip)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(BuiltShip));
            _consumption = constructionDock.WhenAnyValue(r => r.Consumption)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Consumption));

            _isCompleted = constructionDock.WhenAnyValue(r => r.State).Select(r => r == ConstructionDockState.Completed)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(IsCompleted));

            _completionTime = constructionDock.WhenAnyValue(r => r.CompletionTime)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(CompletionTime));
        }
    }
}
