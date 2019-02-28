using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    public partial class HomeportShip
    {
        private Fleet _fleet;
        public Fleet Fleet
        {
            get => _fleet;
            internal set => Set(ref _fleet, value);
        }

        private bool _isRepairing;
        public bool IsRepairing
        {
            get => _isRepairing;
            internal set => Set(ref _isRepairing, value);
        }

        public HomeportSlot ExtraSlot
        {
            get => (HomeportSlot)ExtraEquipment;
            internal set
            {
                ExtraEquipment = value;
                NotifyPropertyChanged();
            }
        }

        private BindableCollection<HomeportSlot> slots = new BindableCollection<HomeportSlot>();
        public override IBindableCollection<Slot> Equipment => slots;
        public IBindableCollection<HomeportSlot> Slots => slots;

        partial void UpdateCore(RawShip raw, DateTimeOffset timeStamp)
        {
            Info = owner.MasterData.ShipInfos[raw.ShipInfoId];
            Fuel = (raw.CurrentFuel, Info.FuelConsumption);
            Bullet = (raw.CurrentBullet, Info.BulletConsumption);
            Firepower = Combine(raw.Firepower, Info.Firepower);
            Torpedo = Combine(raw.Torpedo, Info.Torpedo);
            AntiAir = Combine(raw.AntiAir, Info.AntiAir);
            Armor = Combine(raw.Armor, Info.Armor);
            Luck = Combine(raw.Luck, Info.Luck);

            SlotCount = Info.SlotCount;
            while (slots.Count < SlotCount)
                slots.Add(new HomeportSlot());
            while (slots.Count > SlotCount)
                slots.RemoveAt(slots.Count - 1);

            if (raw.ExtraSlotOpened)
                ExtraSlot = new HomeportSlot
                {
                    Equipment = owner.AllEquipment[raw.ExtraSlotEquipId]
                };
            else
                ExtraSlot = null;

            UpdateEquipmentsCore(raw.EquipmentIds);
            UpdateSlotAircraftCore(raw.SlotAircraft);

            var lineOfSight = 0;
            var evasion = 0;
            var antiSubmarine = 0;

            foreach (var slot in slots)
            {
                var equipment = slot.Equipment?.Info;
                if (equipment == null)
                    continue;

                lineOfSight += equipment.LineOfSight;
                evasion += equipment.Evasion;
                antiSubmarine += equipment.AntiSubmarine;
            }

            LineOfSight = Substract(raw.LineOfSight, lineOfSight);
            Evasion = Substract(raw.Evasion, evasion);
            AntiSubmarine = Substract(raw.AntiSubmarine, antiSubmarine);

            DoCalculations();
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

        internal void SetRepaired()
        {
            using (EnterBatchNotifyScope())
            {
                IsRepairing = false;
                var maxHp = HP.Max;
                HP = (maxHp, maxHp);
                if (Morale < 40)
                    Morale = 40;
                Fleet?.UpdateState();
            }
        }

        internal void Supply(ShipSupply raw)
        {
            using (EnterBatchNotifyScope())
            {
                Fuel = (raw.CurrentFuel, Info.FuelConsumption);
                Bullet = (raw.CurrentBullet, Info.BulletConsumption);
                UpdateSlotAircraftCore(raw.SlotAircraft);
                DoCalculations();
                Fleet?.UpdateState();
            }
        }

        internal void UpdateEquipments(IReadOnlyList<EquipmentId?> equipmentIds)
        {
            using (EnterBatchNotifyScope())
            {
                UpdateEquipmentsCore(equipmentIds);
                DoCalculations();
                Fleet?.UpdateState();
            }
        }

        private void UpdateEquipmentsCore(IReadOnlyList<EquipmentId?> equipmentIds)
        {
            for (int i = 0; i < slots.Count; i++)
                slots[i].Equipment = owner.AllEquipment[equipmentIds[i]];
        }

        private void UpdateSlotAircraftCore(IReadOnlyList<int> aircrafts)
        {
            for (int i = 0; i < slots.Count; i++)
                slots[i].Aircraft = (aircrafts[i], Info.Aircraft[i]);
        }
    }
}
