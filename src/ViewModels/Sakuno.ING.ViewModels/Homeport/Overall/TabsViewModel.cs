namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class TabsViewModel : ReactiveObject, ISelectedFleetStateProvider
{
    private readonly ReadOnlyObservableCollection<FleetTabViewModel> _fleets;
    public ReadOnlyObservableCollection<FleetTabViewModel> Fleets => _fleets;

    public IObservable<FleetId> SelectedFleetId { get; }

    public TabsViewModel(PlayerDataService playerDataService)
    {
        playerDataService.Fleets.Connect()
            .Transform(fleet => new FleetTabViewModel(fleet.Id, this))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _fleets)
            .Subscribe();

        SelectedFleetId = _fleets.ToObservableChangeSet()
            .MergeMany(vm => vm.SelectCommand.Select(_ => vm.Id))
            .Merge(playerDataService.Fleets.Connect().Take(1).Select(_ => (FleetId)1))
            .DistinctUntilChanged();
    }
}
