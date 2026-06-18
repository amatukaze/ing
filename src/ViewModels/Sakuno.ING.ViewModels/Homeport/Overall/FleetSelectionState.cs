using System.Reactive.Subjects;
using Sakuno.ING.Game.Provider;

namespace Sakuno.ING.ViewModels.Homeport.Overall;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class FleetSelectionState
{
    private readonly Subject<FleetId> _selectedId = new();

    public IObservable<FleetId> SelectedId { get; }

    public FleetSelectionState(IGameProvider gameProvider)
    {
        SelectedId = gameProvider.FleetsUpdated
            .Take(1)
            .Select(_ => FleetId.From(1))
            .Merge(_selectedId)
            .DistinctUntilChanged()
            .ObserveOn(RxSchedulers.MainThreadScheduler);
    }

    public void Select(FleetId id) => _selectedId.OnNext(id);
}
