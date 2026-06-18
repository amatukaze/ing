using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport.Overall;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class TabsViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<FleetTabViewModel> _fleets;
    public ReadOnlyObservableCollection<FleetTabViewModel> Fleets => _fleets;

    public TabsViewModel(IPlayerDataService playerDataService, FleetSelectionState fleetSelectionState)
    {
        playerDataService.Fleets.Connect()
            .Transform(fleet => new FleetTabViewModel(fleet.Id, fleetSelectionState))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Bind(out _fleets)
            .Subscribe()
            .DisposeWith(Disposables);
    }
}
