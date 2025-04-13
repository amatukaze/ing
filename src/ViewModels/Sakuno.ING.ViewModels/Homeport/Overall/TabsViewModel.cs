namespace Sakuno.ING.ViewModels.Homeport.Overall;

[RegisterScoped]
public class TabsViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<FleetTabViewModel> _fleets;
    public ReadOnlyObservableCollection<FleetTabViewModel> Fleets => _fleets;

    public TabsViewModel(PlayerDataService playerDataService, FleetSelectionState fleetSelectionState)
    {
        playerDataService.Fleets.Connect()
            .Transform(fleet => new FleetTabViewModel(fleet.Id, fleetSelectionState))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _fleets)
            .Subscribe()
            .DisposeWith(Disposables);
    }
}
