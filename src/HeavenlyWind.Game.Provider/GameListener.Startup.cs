using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Messaging;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    partial class GameListener
    {
        public readonly IProducer<IReadOnlyCollection<IRawEquipment>> AllEquipmentUpdated;
        public readonly IProducer<IReadOnlyCollection<IRawBuildingDock>> BuildingDockUpdated;
        public readonly IProducer<IReadOnlyCollection<IRawUseItemCount>> UseItemUpdated;
        public readonly IProducer<IDictionary<string, int[]>> FreeEquipmentUpdated;
    }
}
