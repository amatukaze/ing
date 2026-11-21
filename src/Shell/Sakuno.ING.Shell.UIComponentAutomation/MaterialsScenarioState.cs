using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Sakuno.ING.ViewModels.Homeport;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal sealed class MaterialsScenarioState : IDisposable
{
    private readonly CompositeDisposable _subscriptions = new();
    private int _fuel;
    private int _bullet;
    private int _steel;
    private int _bauxite;
    private int _instantBuild;
    private int _instantRepair;
    private int _development;
    private int _improvement;

    public void Attach(MaterialsViewModel viewModel)
    {
        viewModel.Fuel.Current.Subscribe(value => _fuel = value).DisposeWith(_subscriptions);
        viewModel.Bullet.Current.Subscribe(value => _bullet = value).DisposeWith(_subscriptions);
        viewModel.Steel.Current.Subscribe(value => _steel = value).DisposeWith(_subscriptions);
        viewModel.Bauxite.Current.Subscribe(value => _bauxite = value).DisposeWith(_subscriptions);
        viewModel.InstantBuild.Current.Subscribe(value => _instantBuild = value).DisposeWith(_subscriptions);
        viewModel.InstantRepair.Current.Subscribe(value => _instantRepair = value).DisposeWith(_subscriptions);
        viewModel.Development.Current.Subscribe(value => _development = value).DisposeWith(_subscriptions);
        viewModel.Improvement.Current.Subscribe(value => _improvement = value).DisposeWith(_subscriptions);
    }

    public IReadOnlyDictionary<string, object?> Capture() =>
        new Dictionary<string, object?>
        {
            ["Fuel"] = _fuel,
            ["Bullet"] = _bullet,
            ["Steel"] = _steel,
            ["Bauxite"] = _bauxite,
            ["InstantBuild"] = _instantBuild,
            ["InstantRepair"] = _instantRepair,
            ["Development"] = _development,
            ["Improvement"] = _improvement,
        };

    public void Dispose() => _subscriptions.Dispose();
}
