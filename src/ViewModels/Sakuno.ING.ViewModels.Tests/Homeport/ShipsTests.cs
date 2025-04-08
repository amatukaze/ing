namespace Sakuno.ING.ViewModels.Tests.Homeport;

public class ShipsTests
{
    private readonly Subject<IReadOnlyList<IShipUpdated>> _shipsUpdatedSubject = new();
    private readonly Subject<IReadOnlyList<IShipUpdated>> _partialShipsUpdatedSubject = new();
    private readonly ShipsViewModel _vm;
    private readonly ObservableCollector<int> _count = new();

    public ShipsTests()
    {
        var provider = Substitute.For<IGameProvider>();
        provider.ShipsUpdated.Returns(_shipsUpdatedSubject);
        provider.PartialShipsUpdated.Returns(_partialShipsUpdatedSubject);

        _vm = new ShipsViewModel(new PlayerDataService(provider));
        _vm.Count.Subscribe(_count);
    }

    [Fact]
    public void ZeroCountAtFirst()
    {
        Assert.Equal(0, _count.LatestValue);
    }

    [Fact]
    public void FullUpdate()
    {
        _shipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(1, 4).ToArray());

        Assert.Equal(4, _count.LatestValue);

        _shipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(10, 6).ToArray());

        Assert.Equal(6, _count.LatestValue);
    }

    [Fact]
    public void PartialUpdate()
    {
        _partialShipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(1, 3).ToArray());

        Assert.Equal(3, _count.LatestValue);

        _partialShipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(2, 5).ToArray());

        Assert.Equal(6, _count.LatestValue);
    }
}
