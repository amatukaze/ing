namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped]
public class ConstructionDocksViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<ConstructionDockId> _constructionDocks;
    public ReadOnlyObservableCollection<ConstructionDockId> ConstructionDocks => _constructionDocks;

    public ConstructionDocksViewModel(PlayerDataService playerDataService)
    {
        playerDataService.ConstructionDocks.Connect()
            .Transform(dock => dock.Id)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _constructionDocks)
            .Subscribe();
    }
}
