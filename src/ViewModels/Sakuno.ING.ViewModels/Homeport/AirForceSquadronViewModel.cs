using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class AirForceSquadronViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<SlotItem> _slotItem;
        public SlotItem SlotItem => _slotItem.Value;

        public AirForceSquadronViewModel(AirForceSquadron squadron)
        {
            _slotItem = squadron.WhenAnyValue(r => r.SlotItem).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(SlotItem));
        }
    }
}
