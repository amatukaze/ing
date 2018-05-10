using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    partial class Ship
    {
        partial void UpdateCore(IRawShip raw)
        {
            Info = shipInfoTable[raw.ShipInfoId];
            Fuel = (raw.CurrentFuel, Info.FuelConsumption);
            Bullet = (raw.CurrentBullet, Info.BulletConsumption);
            Firepower = Combine(raw.Firepower, Info.Firepower);
            Torpedo = Combine(raw.Torpedo, Info.Torpedo);
            AntiAir = Combine(raw.AntiAir, Info.AntiAir);
            Armor = Combine(raw.Armor, Info.Armor);
            Luck = Combine(raw.Luck, Info.Luck);

            SlotCount = Info.SlotCount;
            while (slots.Count < SlotCount)
                slots.Add(new Slot());
            while (slots.Count > SlotCount)
                slots.RemoveAt(slots.Count - 1);

            if (raw.ExtraSlotOpened)
                ExtraSlot = new Slot
                {
                    Equipment = equipmentTable.TryGetOrDummy(raw.ExtraSlotEquipId)
                };
            else
                ExtraSlot = null;

            UpdateEquipments(raw.EquipmentIds);
            UpdateSlotAircraft(raw.SlotAircraft);
            LightOfSight = Substract(raw.LightOfSight, slots.Sum(s => s.Equipment?.Info.LightOfSight ?? 0));
            Evasion = Substract(raw.Evasion, slots.Sum(s => s.Equipment?.Info.Evasion ?? 0));
            AntiSubmarine = Substract(raw.AntiSubmarine, slots.Sum(s => s.Equipment?.Info.AntiSubmarine ?? 0));
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

        private BindableCollection<Slot> slots = new BindableCollection<Slot>();
        public IReadOnlyList<Slot> Slots => slots;

        internal void SetRepaired()
        {
            IsRepairing = false;
            var maxHp = HP.Max;
            HP = (maxHp, maxHp);
            if (Morale < 40)
                Morale = 40;
        }

        internal void Supply(IShipSupply raw)
        {
            Fuel = (raw.CurrentFuel, Info.FuelConsumption);
            Bullet = (raw.CurrentBullet, Info.BulletConsumption);
            UpdateSlotAircraft(raw.SlotAircraft);
        }

        internal void UpdateEquipments(IReadOnlyList<int> equipmentIds)
        {
            for (int i = 0; i < slots.Count; i++)
                slots[i].Equipment = equipmentTable.TryGetOrDummy(equipmentIds[i]);
        }

        internal void UpdateSlotAircraft(IReadOnlyList<int> aircrafts)
        {
            for (int i = 0; i < slots.Count; i++)
                slots[i].Aircraft = (aircrafts[i], Info.Aircraft[i]);
        }
    }
}
