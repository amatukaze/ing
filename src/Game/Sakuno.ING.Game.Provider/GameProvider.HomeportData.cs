using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using System;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<MasterDataUpdate> MasterDataUpdated { get; private set; }

        public IObservable<RawAdmiral> AdmiralUpdated { get; private set; }

        public IObservable<RawSlotItem[]> SlotItemsUpdated{ get; private set; }
        public IObservable<RawConstructionDock[]> ConstructionDocksUpdated { get; private set; }
        public IObservable<RawUseItemCount[]> UseItemsUpdated { get; private set; }
        public IObservable<RawUnequippedSlotItemInfo[]> UnequippedSlotItemInfoUpdated { get; private set; }

        public IObservable<RawShip[]> ShipsUpdated { get; private set; }
        public IObservable<RawRepairDock[]> RepairDocksUpdated { get; private set; }
        public IObservable<RawFleet[]> FleetsUpdated { get; private set; }

        public IObservable<RawShip> ShipUpdated { get; private set; }
        public IObservable<RawSlotItem> SlotItemUpdated { get; private set; }
        public IObservable<RawFleet> FleetUpdated { get; private set; }

        public IObservable<IMaterialUpdate> MaterialUpdated { get; private set; }

        public IObservable<ShipId> ShipLocked { get; private set; }
        public IObservable<ShipId> ShipUnlocked { get; private set; }

        public IObservable<SlotItemId> SlotItemLocked { get; private set; }
        public IObservable<SlotItemId> SlotItemUnlocked { get; private set; }
    }
}
