using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class MaterialsViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<int> _fuel;
        public int Fuel => _fuel.Value;

        private readonly ObservableAsPropertyHelper<int> _bullet;
        public int Bullet => _bullet.Value;

        private readonly ObservableAsPropertyHelper<int> _steel;
        public int Steel => _steel.Value;

        private readonly ObservableAsPropertyHelper<int> _bauxite;
        public int Bauxite => _bauxite.Value;

        private readonly ObservableAsPropertyHelper<int> _instantBuild;
        public int InstantBuild => _instantBuild.Value;

        private readonly ObservableAsPropertyHelper<int> _instantRepair;
        public int InstantRepair => _instantRepair.Value;

        private readonly ObservableAsPropertyHelper<int> _development;
        public int Development => _development.Value;

        private readonly ObservableAsPropertyHelper<int> _improvement;
        public int Improvement => _improvement.Value;

        public MaterialsViewModel(NavalBase navalBase)
        {
            _fuel = navalBase.MaterialsUpdated.Select(r => r.Fuel)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Fuel));
            _bullet = navalBase.MaterialsUpdated.Select(r => r.Bullet)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Bullet));
            _steel = navalBase.MaterialsUpdated.Select(r => r.Steel)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Steel));
            _bauxite = navalBase.MaterialsUpdated.Select(r => r.Bauxite)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Bauxite));
            _instantBuild = navalBase.MaterialsUpdated.Select(r => r.InstantBuild)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(InstantBuild));
            _instantRepair = navalBase.MaterialsUpdated.Select(r => r.InstantRepair)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(InstantRepair));
            _development = navalBase.MaterialsUpdated.Select(r => r.Development)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Development));
            _improvement = navalBase.MaterialsUpdated.Select(r => r.Improvement)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Improvement));
        }
    }
}
