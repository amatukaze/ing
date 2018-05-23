using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawEquipment>> allEquipmentUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawEquipment>> AllEquipmentUpdated
        {
            add => allEquipmentUpdated.Received += value;
            remove => allEquipmentUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawBuildingDock>> buildingDockUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawBuildingDock>> BuildingDockUpdated
        {
            add => buildingDockUpdated.Received += value;
            remove => buildingDockUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<IRawUseItemCount>> useItemUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<IRawUseItemCount>> UseItemUpdated
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
