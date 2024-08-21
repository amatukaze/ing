using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.ViewModels.Tests.Overall;

public class SelectedFleetTests
{
    private readonly Subject<IReadOnlyList<IShipUpdated>> _shipsUpdatedSubject = new();
    private readonly Subject<IReadOnlyList<IFleetUpdated>> _fleetsUpdatedSubject = new();
    private readonly SelectedFleetViewModel _vm;

    private readonly Subject<FleetId> _selectedId = new();

    public SelectedFleetTests()
    {
        var gameProvider = Substitute.For<IGameProvider>();
        gameProvider.ShipsUpdated.Returns(_shipsUpdatedSubject);
        gameProvider.FleetsUpdated.Returns(_fleetsUpdatedSubject);

        var selectedFleetStateProvider = Substitute.For<ISelectedFleetStateProvider>();
        selectedFleetStateProvider.SelectedFleetId.Returns(_selectedId);

        _vm = new SelectedFleetViewModel(new PlayerDataService(gameProvider), selectedFleetStateProvider);
    }

    [Fact]
    public void Test()
    {
        _shipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(1, 4).ToArray());

        var fleet1 = Utils.GenerateMock<IFleetUpdated, FleetId>(1);
        fleet1.Ships.Returns([(ShipId)1, (ShipId)2, (ShipId)3], [(ShipId)1, (ShipId)2, (ShipId)4]);

        var fleet2 = Utils.GenerateMock<IFleetUpdated, FleetId>(2);
        fleet2.Ships.Returns([(ShipId)5]);

        _fleetsUpdatedSubject.OnNext([fleet1, fleet2]);
        _selectedId.OnNext((FleetId)1);

        Assert.Equal([(ShipId)1, (ShipId)2, (ShipId)3], (IReadOnlyList<ShipId>)_vm.Ships.ToArray());

        _fleetsUpdatedSubject.OnNext([fleet1, fleet2]);

        Assert.Equal([(ShipId)1, (ShipId)2, (ShipId)4], (IReadOnlyList<ShipId>)_vm.Ships.ToArray());

        _selectedId.OnNext((FleetId)2);

        Assert.Equal([(ShipId)5], (IReadOnlyList<ShipId>)_vm.Ships.ToArray());
    }
}
