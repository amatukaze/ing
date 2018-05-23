using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public interface IShipSupply
    {
        ShipId ShipId { get; }
        int CurrentFuel { get; }
        int CurrentBullet { get; }
        IReadOnlyList<int> SlotAircraft { get; }
    }
}
