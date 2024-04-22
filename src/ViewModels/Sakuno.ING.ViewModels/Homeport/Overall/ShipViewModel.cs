namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class ShipViewModel : ReactiveObject, IDisposable
{
    public ShipId Id { get; }

    private readonly ObservableAsPropertyHelper<ShipInfoId> _masterId;
    public ShipInfoId MasterId => _masterId.Value;

    private readonly CompositeDisposable _disposables = [];

    public ShipViewModel(ShipId id, PlayerDataService playerDataService)
    {
        Id = id;

        var model = playerDataService.Ships[id];

        _masterId = model.WhenAnyValue(s => s.MasterId).BindProperty(this, nameof(MasterId)).DisposeWith(_disposables);
    }

    public void Dispose() => _disposables.Dispose();
}
