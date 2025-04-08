namespace Sakuno.ING.ViewModels.Homeport.Overall;

[RegisterScoped]
public class SelectedFleetViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<ShipViewModel> _ships;
    public ReadOnlyObservableCollection<ShipViewModel> Ships => _ships;

    public SelectedFleetViewModel(PlayerDataService playerDataService, FleetSelectionState fleetSelectionState)
    {
        ObservableChangeSet.Create<ShipId>(list => fleetSelectionState.SelectedId
                .Select(id => playerDataService.Fleets[id]!.WhenAnyValue(f => f.Ships))
                .Switch()
                .Subscribe(items => list.EditDiff(items.Where(id => id > 0))))
            .Transform(id => new ShipViewModel(id, playerDataService))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _ships)
            .Subscribe();
    }
}
