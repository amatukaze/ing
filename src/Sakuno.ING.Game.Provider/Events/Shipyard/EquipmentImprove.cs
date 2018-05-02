using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class EquipmentImprove
    {
        public int EquipmentId { get; internal set; }
        public int RecipeId { get; internal set; }
        public bool GuaranteedSuccess { get; internal set; }
        public bool IsSuccess { get; internal set; }
        public IRawEquipment UpdatedTo { get; internal set; }
        public IReadOnlyCollection<int> ConsumedEquipmentId { get; internal set; }
    }
}
