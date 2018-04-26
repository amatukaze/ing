using System.Linq;

namespace Sakuno.ING.Game.Models
{
    partial class Ship
    {
        partial void UpdateCore(IRawShip raw)
        {
            Info = shipInfoTable[raw.ShipInfoId];
            Fuel = new ClampedValue(raw.CurrentFuel, Info.FuelConsumption);
            Bullet = new ClampedValue(raw.CurrentBullet, Info.BulletConsumption);
            Firepower = Combine(raw.Firepower, Info.Firepower);
            Torpedo = Combine(raw.Torpedo, Info.Torpedo);
            AntiAir = Combine(raw.AntiAir, Info.AntiAir);
            Armor = Combine(raw.Armor, Info.Armor);
            Luck = Combine(raw.Luck, Info.Luck);

            SlotCount = Info.SlotCount;
            if (SlotCount != _slots?.Length)
            {
                _slots = new Slot[SlotCount];
                for (int i = 0; i < _slots.Length; i++)
                    _slots[i] = new Slot();
                _bindableSlots = null;
                NotifyPropertyChanged(nameof(Slots));
            }

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Equipment = equipmentTable.TryGetOrDummy(raw.EquipmentIds[i]);
                _slots[i].Aircraft = new ClampedValue(raw.SlotAircraft[i], Info.Aircraft[i]);
            }
            LightOfSight = Substract(raw.LightOfSight, _slots.Sum(s => s.Equipment?.Info.LightOfSight ?? 0));
            Evasion = Substract(raw.Evasion, _slots.Sum(s => s.Equipment?.Info.Evasion ?? 0));
            AntiSubmarine = Substract(raw.AntiSubmarine, _slots.Sum(s => s.Equipment?.Info.AntiSubmarine ?? 0));
        }

        private static ShipMordenizationStatus Combine(ShipMordenizationStatus current, ShipMordenizationStatus master)
            => new ShipMordenizationStatus
            {
                Min = master.Min,
                Max = master.Max,
                Displaying = current.Displaying,
                Improved = current.Improved
            };

        private static ShipMordenizationStatus Substract(ShipMordenizationStatus current, int equipment)
            => new ShipMordenizationStatus
            {
                Min = current.Displaying - equipment - current.Improved,
                Max = current.Max,
                Improved = current.Improved,
                Displaying = current.Displaying
            };

        private Slot[] _slots;
        private IBindableCollection<Slot> _bindableSlots;
        public IBindableCollection<Slot> Slots => _bindableSlots ?? (_bindableSlots = _slots.AsBindable());
    }
}
