using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameProvider
    {
        private readonly ITimedMessageProvider<IReadOnlyCollection<RawEquipment>> allEquipmentUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawEquipment>> AllEquipmentUpdated
        {
            add => allEquipmentUpdated.Received += value;
            remove => allEquipmentUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<RawBuildingDock>> buildingDockUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawBuildingDock>> BuildingDockUpdated
        {
            add => buildingDockUpdated.Received += value;
            remove => buildingDockUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<RawUseItemCount>> useItemUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawUseItemCount>> UseItemUpdated
        {
            add => useItemUpdated.Received += value;
            remove => useItemUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyDictionary<string, int[]>> freeEquipmentUpdated;
        public event TimedMessageHandler<IReadOnlyDictionary<string, int[]>> FreeEquipmentUpdated
        {
            add => freeEquipmentUpdated.Received += value;
            remove => freeEquipmentUpdated.Received -= value;
        }
    }
}
