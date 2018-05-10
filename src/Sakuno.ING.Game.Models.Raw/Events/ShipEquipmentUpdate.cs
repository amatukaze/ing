using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
    public class ShipEquipmentUpdate
    {
        public ShipEquipmentUpdate(int shipId, IReadOnlyList<int> equipmentIds)
        {
            ShipId = shipId;
            EquipmentIds = equipmentIds;
        }

        public int ShipId { get; }
        public IReadOnlyList<int> EquipmentIds { get; }
    }
}
