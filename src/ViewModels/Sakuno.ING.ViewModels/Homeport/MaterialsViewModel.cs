namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class MaterialsViewModel : ViewModelObject
{
    public IObservable<Materials> Materials { get; }
    public IObservable<Materials> MaterialsDiff { get; }

    public MaterialsViewModel(PlayerDataService playerDataService)
    {
        Materials = playerDataService.Materials.ObserveOn(RxApp.MainThreadScheduler);
        MaterialsDiff = playerDataService.Materials.Scan(new DiffCache(), (cache, materials) =>
        {
            cache.Previous = cache.Current;
            cache.Current = materials;

            return cache;
        }).Select(cache => new Materials()
        {
            Fuel = cache.Current.Fuel - cache.Previous.Fuel,
            Bullet = cache.Current.Bullet - cache.Previous.Bullet,
            Steel = cache.Current.Steel - cache.Previous.Steel,
            Bauxite = cache.Current.Bauxite - cache.Previous.Bauxite,
            InstantBuild = cache.Current.InstantBuild - cache.Previous.InstantBuild,
            InstantRepair = cache.Current.InstantRepair - cache.Previous.InstantRepair,
            Development = cache.Current.Development - cache.Previous.Development,
            Improvement = cache.Current.Improvement - cache.Previous.Improvement,
        }).ObserveOn(RxApp.MainThreadScheduler);
    }

    private class DiffCache
    {
        public Materials Previous { get; set; }
        public Materials Current { get; set; }
    }
}
