using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed record ShipsDismantled(ShipId[] ShipIds, bool RemoveSlotItems)
    {
    }
}
