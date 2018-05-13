using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipPowerup
    {
        public ShipPowerup(ShipId shipId, IReadOnlyCollection<ShipId> consumedShipIds, bool isSuccess, IRawShip updatedTo)
        {
            ShipId = shipId;
            ConsumedShipIds = consumedShipIds;
            IsSuccess = isSuccess;
            UpdatedTo = updatedTo;
        }

        public ShipId ShipId { get; }
        public IReadOnlyCollection<ShipId> ConsumedShipIds { get; }
        public bool IsSuccess { get; }
        public IRawShip UpdatedTo { get; }
    }
}
