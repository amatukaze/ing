using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class FleetsViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<FleetViewModel> _fleets;
    public ReadOnlyObservableCollection<FleetViewModel> Fleets => _fleets;

    public FleetsViewModel(IPlayerDataService playerDataService)
    {
        playerDataService.Fleets.Connect()
            .Transform(fleet => new FleetViewModel(fleet))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Bind(out _fleets)
            .Subscribe()
            .DisposeWith(Disposables);
    }
}
