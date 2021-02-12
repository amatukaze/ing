using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class SlotViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<SlotItem?> _item;
        public SlotItem? Item => _item.Value;

        private readonly ObservableAsPropertyHelper<ClampedValue> _planeCount;
        public ClampedValue PlaneCount => _planeCount.Value;

        private readonly ObservableAsPropertyHelper<bool> _isPlaneCountVisible;
        public bool IsPlaneCountVisible => _isPlaneCountVisible.Value;

        public SlotViewModel(PlayerShipSlot slot)
        {
            _item = slot.WhenAnyValue(r => r.Item).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(Item));

            var planeCount = slot.WhenAnyValue(r => r.PlaneCount);

            _planeCount = planeCount.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(PlaneCount));
            _isPlaneCountVisible = planeCount.Select(r => r.Max > 0)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(IsPlaneCountVisible));
        }
    }
}
