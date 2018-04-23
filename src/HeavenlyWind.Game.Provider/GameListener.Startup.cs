using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Messaging;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawEquipment>>> AllEquipmentUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawBuildingDock>>> BuildingDockUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawUseItemCount>>> UseItemUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyDictionary<string, int[]>>> FreeEquipmentUpdated;
    }
}
