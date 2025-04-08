using System.Reactive.Subjects;

namespace Sakuno.ING.ViewModels.Homeport.Overall;

[RegisterScoped]
public class FleetSelectionState
{
    private readonly Subject<FleetId> _selectedId = new();

    public IObservable<FleetId> SelectedId { get; }

    public FleetSelectionState(PlayerDataService playerDataService)
    {
        SelectedId = playerDataService.Fleets.Connect().Take(1).Select(_ => (FleetId)1).Merge(_selectedId)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler);
    }

    public void Select(FleetId id) => _selectedId.OnNext(id);
}
