using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using System;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<MasterDataUpdate> MasterDataUpdated { get; private set; }

        public IObservable<RawSlotItem[]> SlotItemUpdated{ get; private set; }
        public IObservable<RawConstructionDock[]> ConstructionDockUpdated { get; private set; }
        public IObservable<RawUseItemCount[]> UseItemUpdated { get; private set; }
        public IObservable<RawUnequippedSlotItemInfo[]> UnequippedSlotItemInfoUpdated { get; private set; }
    }
}
