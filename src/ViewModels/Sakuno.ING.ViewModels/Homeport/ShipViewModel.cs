using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class ShipViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<ShipInfo> _info;
        public ShipInfo Info => _info.Value;

        private readonly ObservableAsPropertyHelper<int> _level;
        public int Level => _level.Value;

        private readonly ObservableAsPropertyHelper<int> _nextExperience;
        public int NextExperience => _nextExperience.Value;

        private readonly ObservableAsPropertyHelper<int> _currentHP;
        public int CurrentHP => _currentHP.Value;

        private readonly ObservableAsPropertyHelper<int> _maxHP;
        public int MaxHP => _maxHP.Value;

        public IReadOnlyCollection<SlotViewModel> Slots { get; }

        public ShipViewModel(PlayerShip ship)
        {
            _info = ship.WhenAnyValue(r => r.Info)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Info));

            var leveling = ship.WhenAnyValue(r => r.Leveling);

            _level = leveling.Select(r => r.Level)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Level));
            _nextExperience = leveling.Select(r => r.ExperienceRemaining)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(NextExperience));

            var hp = ship.WhenAnyValue(r => r.HP);

            _currentHP = hp.Select(r => r.Current)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(CurrentHP));
            _maxHP = hp.Select(r => r.Max)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(MaxHP));

            Slots = ship.Slots.AsObservableChangeSet().Transform(r => new SlotViewModel(r)).Bind();
        }
    }
}
