using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class AdmiralViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<string> _name;
        public string Name => _name.Value;

        private readonly ObservableAsPropertyHelper<int> _level;
        public int Level => _level.Value;

        public AdmiralViewModel(NavalBase navalBase)
        {
            _name = navalBase.AdmiralUpdated.Select(r => r.Name)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Name));
            _level = navalBase.AdmiralUpdated.Select(r => r.Leveling.Level)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Level));
        }
    }
}
