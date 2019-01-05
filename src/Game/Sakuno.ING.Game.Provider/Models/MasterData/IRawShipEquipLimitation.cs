using System.Collections.Generic;

namespace Sakuno.ING.Game.Models.MasterData
{
    public interface IRawShipEquipLimitation : IIdentifiable<ShipId>
    {
        IReadOnlyCollection<EquipmentTypeId> AvailableEquipmentTypes { get; }
    }
}
