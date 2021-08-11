using DynamicData.Aggregation;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Homeport
{
    [Export]
    public sealed class OverallViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<int> _shipCount;
        public int ShipCount => _shipCount.Value;

        private readonly ObservableAsPropertyHelper<int> _maxShipCount;
        public int MaxShipCount => _maxShipCount.Value;

        private readonly ObservableAsPropertyHelper<int> _slotItemCount;
        public int SlotItemCount => _slotItemCount.Value;

        private readonly ObservableAsPropertyHelper<int> _maxSlotItemCount;
        public int MaxSlotItemCount => _maxSlotItemCount.Value;

        public OverallViewModel(NavalBase navalBase)
        {
            _shipCount = navalBase.Ships.DefaultViewSource.Count()
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(ShipCount));
            _maxShipCount = navalBase.AdmiralUpdated.Select(r => r.MaxShipCount)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(MaxShipCount));

            _slotItemCount = navalBase.SlotItems.DefaultViewSource.Count()
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(SlotItemCount));
            _maxSlotItemCount = navalBase.AdmiralUpdated.Select(r => r.MaxSlotItemCount)
                .ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(MaxSlotItemCount));
        }
    }
}
