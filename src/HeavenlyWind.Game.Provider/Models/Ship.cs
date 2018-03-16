using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.KanColle.Amatsukaze.Game.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class Ship : ManualNotifyObject, IIdentifiable, IBindableWithChildren<Slot>
    {
        public int Id { get; protected set; }

        public ShipInfo Info { get; protected set; }

        public Leveling Leveling { get; protected set; }
        public ClampedValue HP { get; protected set; }

        public ShipSpeed Speed { get; protected set; }
        public FireRange FireRange { get; protected set; }

        public IBindableCollection<Slot> Slots { get; protected set; }
        public Slot ExtraSlot { get; protected set; }
        public IEnumerable<Slot> AllSlots => ExtraSlot == null ? Slots : Slots.Append(ExtraSlot);

        public ClampedValue Fuel { get; protected set; }
        public ClampedValue Bullet { get; protected set; }

        public TimeSpan RepairTime { get; protected set; }
        public int RepiarFuelConsumption { get; protected set; }
        public int RepairSteelConsumption { get; protected set; }

        public int Condition { get; protected set; }

        public ShipMordenizationStatus Firepower { get; protected set; }
        public ShipMordenizationStatus Torpedo { get; protected set; }
        public ShipMordenizationStatus AntiAir { get; protected set; }
        public ShipMordenizationStatus Armor { get; protected set; }
        public ShipMordenizationStatus AntiSubmarine { get; protected set; }
        public ShipMordenizationStatus Evasion { get; protected set; }
        public ShipMordenizationStatus LightOfSight { get; protected set; }
        public ShipMordenizationStatus Luck { get; protected set; }

        public bool IsLocked { get; protected set; }

        IBindableCollection<Slot> IBindableWithChildren<Slot>.Children => Slots;
        public event Action<IBindableCollection<Slot>> ChildrenChanged;

        protected override void NotifyAllPropertyChanged()
        {
            ChildrenChanged?.Invoke(Slots);
            base.NotifyAllPropertyChanged();
        }
    }
}
