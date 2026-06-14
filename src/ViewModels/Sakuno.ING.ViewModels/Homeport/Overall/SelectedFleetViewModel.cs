namespace Sakuno.ING.ViewModels.Homeport.Overall;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class SelectedFleetViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<ShipViewModel> _ships;
    public ReadOnlyObservableCollection<ShipViewModel> Ships => _ships;

    public SelectedFleetViewModel(PlayerDataService playerDataService, FleetSelectionState fleetSelectionState)
    {
        ObservableChangeSet.Create<ShipId>(list => fleetSelectionState.SelectedId
                .Select(id => playerDataService.Fleets.Snapshot[id].WhenAnyValue(f => f.Ships))
                .Switch()
                .Subscribe(items => list.EditDiff(items.Where(id => id.IsValid))))
            .Transform(id => new ShipViewModel(id, playerDataService))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Bind(out _ships)
            .Subscribe()
            .DisposeWith(Disposables);
    }
}
