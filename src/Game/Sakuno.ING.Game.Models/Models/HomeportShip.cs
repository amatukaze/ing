using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Models
{
    public partial class HomeportShip
    {
        public override Slot ExtraSlot => ExtraHomeportSlot;
        private HomeportSlot _extraHomeportSlot;
        public HomeportSlot ExtraHomeportSlot
        {
            get => _extraHomeportSlot;
            private set
            {
                if (_extraHomeportSlot != value)
                {
                    _extraHomeportSlot = value;
                    using (EnterBatchNotifyScope())
                    {
                        NotifyPropertyChanged();
                        NotifyPropertyChanged(nameof(ExtraSlot));
                    }
                }
            }
        }

        private readonly BindableCollection<HomeportSlot> slots = new BindableCollection<HomeportSlot>();
        public override IBindableCollection<Slot> Slots => slots;
        public IBindableCollection<HomeportSlot> HomeportSlots => slots;

        internal IEnumerable<HomeportEquipment> AllEquipped => slots
            .Append(ExtraHomeportSlot)
            .Select(x => x?.HomeportEquipment)
            .Where(x => x != null);

        partial void UpdateCore(RawShip raw, DateTimeOffset timeStamp)
        {
            static ShipMordenizationStatus Combine(ShipMordenizationStatus current, ShipMordenizationStatus master)
               => new ShipMordenizationStatus
               (
                   min: master.Min,
                   max: master.Max,
                   displaying: current.Displaying,
                   improved: current.Improved
               );
            static ShipMordenizationStatus Substract(ShipMordenizationStatus current, int equipment)
                => new ShipMordenizationStatus
                (
                    min: current.Displaying - equipment - current.Improved,
                    max: current.Max,
                    improved: current.Improved,
                    displaying: current.Displaying
                );

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
                slots.Add(new HomeportSlot(this, slots.Count));
            while (slots.Count > SlotCount)
            {
                // TODO: log fatal inconsistence
                slots[slots.Count - 1].Destroy();
                slots.RemoveAt(slots.Count - 1);
            }

            if (raw.ExtraSlotOpened && ExtraHomeportSlot == null)
                ExtraHomeportSlot = new HomeportSlot(this, -1);
            else if (!raw.ExtraSlotOpened && ExtraHomeportSlot != null)
            {
                // TODO: log fatal inconsistence
                ExtraHomeportSlot?.Destroy();
                ExtraHomeportSlot = null;
            }

            if (ExtraHomeportSlot != null)
            {
                ExtraHomeportSlot.HomeportEquipment = owner.AllEquipment[raw.ExtraSlotEquipId];
            }

            var lineOfSight = 0;
            var evasion = 0;
            var antiSubmarine = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                slot.Aircraft = (raw.SlotAircraft[i], Info.Aircraft[i]);
                var equipment = slot.HomeportEquipment = owner.AllEquipment[raw.EquipmentIds[i]];
                slot.DoCalculations();

                var info = equipment?.Info;
                if (info is null)
                    continue;

                lineOfSight += info.LineOfSight;
                evasion += info.Evasion;
                antiSubmarine += info.AntiSubmarine;
            }

            LineOfSight = Substract(raw.LineOfSight, lineOfSight);
            Evasion = Substract(raw.Evasion, evasion);
            AntiSubmarine = Substract(raw.AntiSubmarine, antiSubmarine);

            CascadeUpdate();
        }

        internal void OpenExtraSlot()
        {
            ExtraHomeportSlot = new HomeportSlot(this, -1);
            CascadeUpdate();
        }

        internal void SetRepaired()
        {
            using (EnterBatchNotifyScope())
            {
                IsRepairing = false;
                RepairingCost = default;
                var maxHp = HP.Max;
                HP = (maxHp, maxHp);
                if (Morale < 40)
                    Morale = 40;
                CascadeUpdate();
            }
        }

        internal void Supply(ShipSupply raw)
        {
            using (EnterBatchNotifyScope())
            {
                Fuel = (raw.CurrentFuel, Info.FuelConsumption);
                Bullet = (raw.CurrentBullet, Info.BulletConsumption);
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].Aircraft = (raw.SlotAircraft[i], Info.Aircraft[i]);
                    slots[i].DoCalculations();
                }
                CascadeUpdate();
            }
        }

        internal void CascadeUpdate()
        {
            DoCalculations();
            Fleet?.CascadeUpdate();
        }
    }
}
