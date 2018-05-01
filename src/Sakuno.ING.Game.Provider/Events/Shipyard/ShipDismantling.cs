using System.Collections.Generic;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipDismantling
    {
        public IReadOnlyCollection<int> ShipIds { get; internal set; }
        public bool DismantleEquipments { get; internal set; }
    }
}
