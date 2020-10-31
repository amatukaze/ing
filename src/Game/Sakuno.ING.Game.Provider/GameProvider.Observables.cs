using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using System;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<MasterDataUpdate> MasterDataUpdated { get; private set; }

        public IObservable<RawSlotItem[]> SlotItemsUpdated{ get; private set; }
        public IObservable<RawConstructionDock[]> ConstructionDocksUpdated { get; private set; }
        public IObservable<RawUseItemCount[]> UseItemsUpdated { get; private set; }
        public IObservable<RawUnequippedSlotItemInfo[]> UnequippedSlotItemInfoUpdated { get; private set; }

        public IObservable<RawShip[]> ShipsUpdate { get; private set; }
        public IObservable<RawRepairDock[]> RepairDocksUpdate { get; private set; }
        public IObservable<RawFleet[]> FleetsUpdate { get; private set; }

        public IObservable<IMaterialUpdate> MaterialUpdate { get; private set; }
    }
}
