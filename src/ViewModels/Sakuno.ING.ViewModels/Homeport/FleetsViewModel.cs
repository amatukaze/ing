namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped]
public class FleetsViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<FleetViewModel> _fleets;
    public ReadOnlyObservableCollection<FleetViewModel> Fleets => _fleets;

    public FleetsViewModel(PlayerDataService playerDataService)
    {
        playerDataService.Fleets.Connect()
            .Transform(fleet => new FleetViewModel(fleet))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _fleets)
            .Subscribe()
            .DisposeWith(Disposables);
    }
}
