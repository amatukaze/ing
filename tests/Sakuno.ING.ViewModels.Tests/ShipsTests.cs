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

        shipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(1, 3).ToArray());

        Assert.Equal(3, vm.Count);

        partialShipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(4, 2).ToArray());

        Assert.Equal(5, vm.Count);
    }
}
