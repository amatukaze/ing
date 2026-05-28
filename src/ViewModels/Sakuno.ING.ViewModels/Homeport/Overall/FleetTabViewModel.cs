using System.Reactive;

namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class FleetTabViewModel : ViewModelObject
{
    public FleetId Id { get; }

    public IObservable<bool> IsSelected { get; }

    public ReactiveCommand<Unit, Unit> SelectCommand { get; }

    public FleetTabViewModel(FleetId id, FleetSelectionState fleetSelectionState)
    {
        Id = id;
        SelectCommand = ReactiveCommand.Create(() => fleetSelectionState.Select(Id));

        IsSelected = fleetSelectionState.SelectedId.Select(selectedId => Id == selectedId)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
    }
}
