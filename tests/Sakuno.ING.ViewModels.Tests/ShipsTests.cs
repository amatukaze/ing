using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Provider;
using Sakuno.ING.Game.Services;
using Sakuno.ING.ViewModels.Homeport;

namespace Sakuno.ING.ViewModels.Tests;

public class ShipsTests
{
    [Fact]
    public void Count()
    {
        var shipsUpdatedSubject = new Subject<IReadOnlyList<IShipUpdated>>();
        var partialShipsUpdatedSubject = new Subject<IReadOnlyList<IShipUpdated>>();
        var provider = Substitute.For<IGameProvider>();
        provider.ShipsUpdated.Returns(shipsUpdatedSubject);
        provider.PartialShipsUpdated.Returns(partialShipsUpdatedSubject);

        var vm = new ShipsViewModel(new PlayerDataService(provider));

        Assert.Equal(0, vm.Count);

        shipsUpdatedSubject.OnNext(Utils.GenerateShips(1, 3).ToArray());

        Assert.Equal(3, vm.Count);

        partialShipsUpdatedSubject.OnNext(Utils.GenerateShips(4, 2).ToArray());

        Assert.Equal(5, vm.Count);
    }
}
