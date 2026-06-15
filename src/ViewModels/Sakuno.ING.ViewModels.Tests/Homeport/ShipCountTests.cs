namespace Sakuno.ING.ViewModels.Tests.Homeport;

public class ShipCountTests
{
    private readonly Subject<IReadOnlyList<IShipUpdated>> _shipsUpdatedSubject = new();
    private readonly Subject<IAdmiralUpdated> _admiralUpdatedSubject = new();

    private readonly ObservableCollector<int> _count = new();
    private readonly ObservableCollector<int> _maxCount = new();

    public ShipCountTests()
    {
        var provider = Substitute.For<IGameProvider>();
        provider.ShipsUpdated.Returns(_shipsUpdatedSubject);
        provider.AdmiralUpdated.Returns(_admiralUpdatedSubject);

        var vm = new ShipCountViewModel(new PlayerDataService(provider));
        vm.Count.Subscribe(_count);
        vm.MaxCount.Subscribe(_maxCount);
    }

    [Fact]
    public void CountAndMaxCountStartAtZero()
    {
        Assert.Equal(0, _count.LatestValue);
        Assert.Equal(0, _maxCount.LatestValue);
    }

    [Fact]
    public void CountSetAfterShipsUpdatedEvent()
    {
        _shipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(1, 3).ToArray());

        Assert.Equal(3, _count.LatestValue);
    }

    [Fact]
    public void MaxCountSetAfterAdmiralUpdatedEvent()
    {
        var admiral = Substitute.For<IAdmiralUpdated>();
        admiral.MaxShipCount.Returns(120);

        _admiralUpdatedSubject.OnNext(admiral);

        Assert.Equal(120, _maxCount.LatestValue);
    }
}
