namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class RepairDocksViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<RepairDockViewModel> _repairDocks;
    public ReadOnlyObservableCollection<RepairDockViewModel> RepairDocks => _repairDocks;

    public RepairDocksViewModel(PlayerDataService playerDataService)
    {
        playerDataService.RepairDocks.Connect()
            .Transform(dock => new RepairDockViewModel(dock, playerDataService))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _repairDocks)
            .Subscribe()
            .DisposeWith(Disposables);
    }
}
