using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class MaterialsViewModel : ViewModelObject
{
    public MaterialViewModel Fuel { get; }
    public MaterialViewModel Bullet { get; }
    public MaterialViewModel Steel { get; }
    public MaterialViewModel Bauxite { get; }
    public MaterialViewModel InstantBuild { get; }
    public MaterialViewModel InstantRepair { get; }
    public MaterialViewModel Development { get; }
    public MaterialViewModel Improvement { get; }

    public MaterialsViewModel(IPlayerDataService playerDataService)
    {
        Fuel = new MaterialViewModel(playerDataService.Materials.Select(m => m.Fuel));
        Bullet = new MaterialViewModel(playerDataService.Materials.Select(m => m.Bullet));
        Steel = new MaterialViewModel(playerDataService.Materials.Select(m => m.Steel));
        Bauxite = new MaterialViewModel(playerDataService.Materials.Select(m => m.Bauxite));
        InstantBuild = new MaterialViewModel(playerDataService.Materials.Select(m => m.InstantBuild));
        InstantRepair = new MaterialViewModel(playerDataService.Materials.Select(m => m.InstantRepair));
        Development = new MaterialViewModel(playerDataService.Materials.Select(m => m.Development));
        Improvement = new MaterialViewModel(playerDataService.Materials.Select(m => m.Improvement));
    }
}
