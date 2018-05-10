using System.Collections.Generic;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipDismantling
    {
        public ShipDismantling(IReadOnlyCollection<int> shipIds, bool dismantleEquipments)
        {
            ShipIds = shipIds;
            DismantleEquipments = dismantleEquipments;
        }

        public IReadOnlyCollection<int> ShipIds { get; }
        public bool DismantleEquipments { get; }
    }
}
