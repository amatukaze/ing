using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class SlotViewModel : ReactiveObject
    {
        public PlayerShipSlot Model { get; }

        private readonly ObservableAsPropertyHelper<bool> _isPlaneCountVisible;
        public bool IsPlaneCountVisible => _isPlaneCountVisible.Value;

        public SlotViewModel(PlayerShipSlot slot)
        {
            Model = slot;

            _isPlaneCountVisible = slot.WhenAnyValue(r => r.PlaneCount).Select(r => r.Max > 0).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(IsPlaneCountVisible));
        }
    }
}
