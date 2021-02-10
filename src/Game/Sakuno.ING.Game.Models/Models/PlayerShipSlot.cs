using System;

namespace Sakuno.ING.Game.Models
{
    public sealed class PlayerShipSlot : Slot
    {
        public PlayerShip Owner { get; }
        public int Index { get; }

        public override SlotItem? Item => _slotItem;

        private PlayerSlotItem? _slotItem;
        public PlayerSlotItem? PlayerSlotItem
        {
            get => _slotItem;
            internal set => Set(ref _slotItem, value);
        }

        public PlayerShipSlot(PlayerShip owner, int index)
        {
            Owner = owner;
            Index = index;
        }
    }
}
