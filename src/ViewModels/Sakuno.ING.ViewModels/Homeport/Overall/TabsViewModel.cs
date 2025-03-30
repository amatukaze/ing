namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class TabsViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<FleetTabViewModel> _fleets;
    public ReadOnlyObservableCollection<FleetTabViewModel> Fleets => _fleets;

    public TabsViewModel(PlayerDataService playerDataService, FleetSelectionState fleetSelectionState)
    {
        playerDataService.Fleets.Connect()
            .Transform(fleet => new FleetTabViewModel(fleet.Id, fleetSelectionState))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _fleets)
            .DisposeMany()
            .Subscribe();
    }
}
