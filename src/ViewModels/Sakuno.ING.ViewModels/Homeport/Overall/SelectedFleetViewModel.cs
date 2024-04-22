namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class SelectedFleetViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<ShipId> _ships;
    public ReadOnlyObservableCollection<ShipId> Ships => _ships;

    public SelectedFleetViewModel(PlayerDataService playerDataService, ISelectedFleetStateProvider selectedFleetStateProvider)
    {
        ObservableChangeSet.Create<ShipId>(list => selectedFleetStateProvider.SelectedFleetId
                .Select(id => playerDataService.Fleets[id]!.WhenAnyValue(f => f.Ships))
                .Switch()
                .Subscribe(items => list.EditDiff(items.Where(id => id > 0))))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _ships)
            .Subscribe();
    }
}
