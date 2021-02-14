using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class AirForceGroupViewModel : ReactiveObject
    {
        public IReadOnlyCollection<AirForceSquadronViewModel> Squadrons { get; }

        private readonly ObservableAsPropertyHelper<int> _combatRadius;
        public int CombatRadius => _combatRadius.Value;

        public AirForceGroupViewModel(AirForceGroup airForceGroup)
        {
            Squadrons = airForceGroup.Squadrons.DefaultViewSource.Transform(r => new AirForceSquadronViewModel(r)).Bind();

            _combatRadius = airForceGroup.WhenAnyValue(r => r.BaseCombatRadius, r => r.BonusCombatRadius, (x, y) => x + y)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(CombatRadius));
        }
    }
}
