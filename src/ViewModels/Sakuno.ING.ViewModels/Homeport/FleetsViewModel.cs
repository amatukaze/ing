namespace Sakuno.ING.ViewModels.Homeport;

public class FleetsViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<FleetId> _fleets;
    public ReadOnlyObservableCollection<FleetId> Fleets => _fleets;

    public FleetsViewModel(PlayerDataService playerDataService)
    {
        playerDataService.Fleets.ToObservableChangeSet()
            .Transform(fleet => fleet.Id)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _fleets)
            .Subscribe();
    }
}
