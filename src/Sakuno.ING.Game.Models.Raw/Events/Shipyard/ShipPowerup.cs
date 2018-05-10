using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipPowerup
    {
        public ShipPowerup(int shipId, IReadOnlyCollection<int> consumedShipIds, bool isSuccess, IRawShip shipAfter)
        {
            ShipId = shipId;
            ConsumedShipIds = consumedShipIds;
            IsSuccess = isSuccess;
            ShipAfter = shipAfter;
        }

        public int ShipId { get; }
        public IReadOnlyCollection<int> ConsumedShipIds { get; }
        public bool IsSuccess { get; }
        public IRawShip ShipAfter { get; }
    }
}
