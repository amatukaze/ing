using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Tests;

public static class Utils
{
    public static IEnumerable<IShipUpdated> GenerateShips(int fromId, int count)
    {
        for (var i = fromId; i < fromId + count; i++)
        {
            var ship = Substitute.For<IShipUpdated>();

            ship.Id.Returns((ShipId)i);

            yield return ship;
        }
    }
}
