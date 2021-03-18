using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed record ShipConstructed(RawShip Ship, RawSlotItem[] SlotItems)
    {
    }
}
