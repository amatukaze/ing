using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawEquipment>> AllEquipmentUpdated;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawBuildingDock>> BuildingDockUpdated;
        public readonly ITimedMessageProvider<IReadOnlyCollection<IRawUseItemCount>> UseItemUpdated;
        public readonly ITimedMessageProvider<IReadOnlyDictionary<string, int[]>> FreeEquipmentUpdated;
    }
}
