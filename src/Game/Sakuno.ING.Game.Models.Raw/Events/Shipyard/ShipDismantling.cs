using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public readonly struct ShipDismantling
    {
        public ShipDismantling(IReadOnlyCollection<ShipId> shipIds, bool dismantleEquipments)
        {
            ShipIds = shipIds;
            DismantleEquipments = dismantleEquipments;
        }

        public IReadOnlyCollection<ShipId> ShipIds { get; }
        public bool DismantleEquipments { get; }
    }
}
