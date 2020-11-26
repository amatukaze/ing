using Sakuno.ING.Game.Models.MasterData;
using System;

namespace Sakuno.ING.Game.Models
{
    public class NavalBase : BindableObject
    {
        public MasterDataRoot MasterData { get; }

        private readonly IdTable<ConstructionDockId, ConstructionDock, RawConstructionDock, NavalBase> _constructionDocks;
        public ITable<ConstructionDockId, ConstructionDock> ConstructionDocks => _constructionDocks;

        private readonly IdTable<RepairDockId, RepairDock, RawRepairDock, NavalBase> _repairDocks;
        public ITable<RepairDockId, RepairDock> RepairDocks => _repairDocks;

        private readonly IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase> _useItems;
        public ITable<UseItemId, UseItemCount> UseItems => _useItems;

        private readonly IdTable<SlotItemId, PlayerSlotItem, RawSlotItem, NavalBase> _slotItems;
        public ITable<SlotItemId, PlayerSlotItem> SlotItems => _slotItems;

        public NavalBase(GameProvider provider)
        {
            MasterData = new MasterDataRoot(provider);

            _constructionDocks = new IdTable<ConstructionDockId, ConstructionDock, RawConstructionDock, NavalBase>(this);
            _repairDocks = new IdTable<RepairDockId, RepairDock, RawRepairDock, NavalBase>(this);
            _useItems = new IdTable<UseItemId, UseItemCount, RawUseItemCount, NavalBase>(this);
            _slotItems = new IdTable<SlotItemId, PlayerSlotItem, RawSlotItem, NavalBase>(this);

            provider.ConstructionDocksUpdated.Subscribe(message => _constructionDocks.BatchUpdate(message));
            provider.RepairDocksUpdate.Subscribe(message => _repairDocks.BatchUpdate(message));
            provider.UseItemsUpdated.Subscribe(message => _useItems.BatchUpdate(message));
            provider.SlotItemsUpdated.Subscribe(message => _slotItems.BatchUpdate(message));
        }
    }
}
