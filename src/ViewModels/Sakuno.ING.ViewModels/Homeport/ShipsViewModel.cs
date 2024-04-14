namespace Sakuno.ING.ViewModels.Homeport;

public class ShipsViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<int> _count;
    public int Count => _count.Value;

    public ShipsViewModel(PlayerDataService playerDataService)
    {
        _count = playerDataService.Ships.Connect()
            .Count()
            .BindProperty(this, nameof(Count));
    }
}
