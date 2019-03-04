using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public event TimedMessageHandler<IReadOnlyCollection<RawEquipment>> AllEquipmentUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawBuildingDock>> BuildingDockUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawUseItemCount>> UseItemUpdated;
        public event TimedMessageHandler<IReadOnlyDictionary<string, int[]>> FreeEquipmentUpdated;
    }
}
