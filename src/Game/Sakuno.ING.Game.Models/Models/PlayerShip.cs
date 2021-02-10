using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sakuno.ING.Game.Models
{
    public partial class PlayerShip
    {
        private ObservableCollection<PlayerShipSlot> _slots;
        public IReadOnlyCollection<PlayerShipSlot> Slots { get; private set; }

        private PlayerShipSlot? _extraSlot;
        public PlayerShipSlot? ExtraSlot
        {
            get => _extraSlot;
            private set => Set(ref _extraSlot, value);
        }

        partial void CreateCore()
        {
            _slots = new ObservableCollection<PlayerShipSlot>();
            Slots = new ReadOnlyObservableCollection<PlayerShipSlot>(_slots);
        }

        partial void UpdateCore(RawShip raw)
        {
            static ShipModernizationStatus Combine(ShipModernizationStatus current, ShipModernizationStatus master) =>
                new ShipModernizationStatus
                (
                    min: master.Min,
                    max: master.Max,
                    displaying: current.Displaying,
                    improved: current.Improved
                );
            static ShipModernizationStatus Substract(ShipModernizationStatus current, int slotItemValue) =>
                new ShipModernizationStatus
                (
                    min: current.Displaying - slotItemValue - current.Improved,
                    max: current.Max,
                    improved: current.Improved,
                    displaying: current.Displaying
                );

            Info = _owner.MasterData.ShipInfos[raw.ShipInfoId];

            Fuel = (raw.Fuel, Info.FuelConsumption);
            Bullet = (raw.Bullet, Info.BulletConsumption);
            Firepower = Combine(raw.Firepower, Info.Firepower);
            Torpedo = Combine(raw.Torpedo, Info.Torpedo);
            AntiAir = Combine(raw.AntiAir, Info.AntiAir);
            Armor = Combine(raw.Armor, Info.Armor);
            Luck = Combine(raw.Luck, Info.Luck);

            while (_slots.Count < Info.SlotCount)
                _slots.Add(new PlayerShipSlot(this, _slots.Count));
            while (_slots.Count > Info.SlotCount)
                _slots.RemoveAt(_slots.Count - 1);

            var lineOfSight = 0;
            var evasion = 0;
            var antiSubmarine = 0;

            for (var i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                slot.PlaneCount = (raw.PlaneCount[i], Info.PlaneCapacities[i]);

                var slotItem = slot.PlayerSlotItem = _owner.SlotItems[raw.SlotItemIds[i]];
                var info = slotItem?.Info;
                if (info is null)
                    continue;

                lineOfSight += info.LineOfSight;
                evasion += info.Evasion;
                antiSubmarine += info.AntiSubmarine;
            }

            LineOfSight = Substract(raw.LineOfSight, lineOfSight);
            Evasion = Substract(raw.Evasion, evasion);
            AntiSubmarine = Substract(raw.AntiSubmarine, antiSubmarine);
        }
    }
}
