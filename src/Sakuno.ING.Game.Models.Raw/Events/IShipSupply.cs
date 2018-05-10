using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
    public interface IShipSupply
    {
        int ShipId { get; }
        int CurrentFuel { get; }
        int CurrentBullet { get; }
        IReadOnlyList<int> SlotAircraft { get; }
    }
}
