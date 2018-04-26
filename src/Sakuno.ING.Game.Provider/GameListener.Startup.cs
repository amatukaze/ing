using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawEquipment>>> AllEquipmentUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawBuildingDock>>> BuildingDockUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawUseItemCount>>> UseItemUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyDictionary<string, int[]>>> FreeEquipmentUpdated;
    }
}
