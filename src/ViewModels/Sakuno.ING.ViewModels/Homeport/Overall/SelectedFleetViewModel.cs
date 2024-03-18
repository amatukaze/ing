namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class SelectedFleetViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<ShipId> _ships;
    public ReadOnlyObservableCollection<ShipId> Ships => _ships;

    public SelectedFleetViewModel(PlayerDataService playerDataService, ISelectedFleetStateProvider selectedFleetStateProvider)
    {
        var selectedId = Observable.Merge([
            playerDataService.Fleets.ToObservableChangeSet().Take(1).Select(_ => (FleetId)1),
            selectedFleetStateProvider.SelectedFleetId,
        ]);

        ObservableChangeSet.Create<ShipId>(list => selectedId
                .Select(id => playerDataService.Fleets[id]!.WhenAnyValue(f => f.Ships))
                .Switch()
                .Subscribe(items => list.EditDiff(items)))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _ships)
            .Subscribe();
    }
}
