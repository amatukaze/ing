using System;

namespace Sakuno.ING.Game.Models
{
    public partial class HomeportSlot : Slot
    {
        internal HomeportSlot(HomeportShip ship, int index)
        {
            Ship = ship;
            Index = index;
        }

        public override Equipment Equipment => HomeportEquipment;

        private HomeportEquipment _homeportEquipment;
        public HomeportEquipment HomeportEquipment
        {
            get => _homeportEquipment;
            internal set
            {
                using (EnterBatchNotifyScope())
                {
                    if (_homeportEquipment != value)
                    {
                        if (_homeportEquipment != null)
                            _homeportEquipment.Slot = null;
                        if (value != null)
                            if (value.Slot != null)
                                throw new InvalidOperationException("Equipment slot inconsistent");
                            else
                                value.Slot = this;
                        _homeportEquipment = value;
                        using (EnterBatchNotifyScope())
                        {
                            NotifyPropertyChanged();
                            NotifyPropertyChanged(nameof(Equipment));
                        }
                    }
                }
            }
        }

        public HomeportShip Ship { get; }
        public int Index { get; }

        internal void CascadeUpdate()
        {
            DoCalculations();
            Ship.CascadeUpdate();
        }

        internal void Destroy()
        {
            if (HomeportEquipment != null)
            {
                HomeportEquipment.Slot = null;
                HomeportEquipment = null;
            }
        }
    }
}
