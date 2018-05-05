using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class ShipPowerup
    {
        public int ShipId { get; internal set; }
        public IReadOnlyCollection<int> ConsumedShipIds { get; internal set; }
        public bool IsSuccess { get; internal set; }
        public IRawShip ShipAfter { get; internal set; }
    }
}
