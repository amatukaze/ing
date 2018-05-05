using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
    public class ShipEquipmentUpdate
    {
        public int ShipId { get; internal set; }
        public IReadOnlyList<int> EquipmentIds { get; internal set; }
    }
}
