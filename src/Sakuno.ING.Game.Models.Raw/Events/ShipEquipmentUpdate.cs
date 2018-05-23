using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public class ShipEquipmentUpdate
    {
        public ShipEquipmentUpdate(ShipId shipId, IReadOnlyList<EquipmentId> equipmentIds)
        {
            ShipId = shipId;
            EquipmentIds = equipmentIds;
        }

        public ShipId ShipId { get; }
        public IReadOnlyList<EquipmentId> EquipmentIds { get; }
    }
}
