using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class ConstructionDocksViewModel : ViewModelObject
{
    private readonly ReadOnlyObservableCollection<ConstructionDockViewModel> _constructionDocks;
    public ReadOnlyObservableCollection<ConstructionDockViewModel> ConstructionDocks => _constructionDocks;

    public ConstructionDocksViewModel(IPlayerDataService playerDataService)
    {
        playerDataService.ConstructionDocks.Connect()
            .Transform(dock => new ConstructionDockViewModel(dock))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Bind(out _constructionDocks)
            .Subscribe()
            .DisposeWith(Disposables);;
    }
}
