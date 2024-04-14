namespace Sakuno.ING.ViewModels.Homeport;

public class RepairDocksViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<RepairDockId> _repairDocks;
    public ReadOnlyObservableCollection<RepairDockId> RepairDocks => _repairDocks;

    public RepairDocksViewModel(PlayerDataService playerDataService)
    {
        playerDataService.RepairDocks.Connect()
            .Transform(dock => dock.Id)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _repairDocks)
            .Subscribe();
    }
}
