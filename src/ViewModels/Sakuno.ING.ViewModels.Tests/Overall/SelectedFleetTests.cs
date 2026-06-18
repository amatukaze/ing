using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.ViewModels.Tests.Overall;

public class SelectedFleetTests
{
    private readonly Subject<IReadOnlyList<IShipUpdated>> _shipsUpdatedSubject = new();
    private readonly Subject<IReadOnlyList<IFleetUpdated>> _fleetsUpdatedSubject = new();

    private readonly IFleetUpdated _fleet1;
    private readonly IFleetUpdated _fleet2;

    private readonly SelectedFleetViewModel _vm;
    private readonly FleetSelectionState _state;

    public SelectedFleetTests()
    {
        var gameProvider = Substitute.For<IGameProvider>();
        gameProvider.ShipsUpdated.Returns(_shipsUpdatedSubject);
        gameProvider.FleetsUpdated.Returns(_fleetsUpdatedSubject);

        var playerDataService = new PlayerDataService(gameProvider);
        _state = new FleetSelectionState(gameProvider);
        _vm = new SelectedFleetViewModel(playerDataService, _state);

        _fleet1 = Utils.GenerateMock<IFleetUpdated, FleetId>(1);
        _fleet2 = Utils.GenerateMock<IFleetUpdated, FleetId>(2);

        _shipsUpdatedSubject.OnNext(Utils.GenerateMocks<IShipUpdated, ShipId>(1, 10).ToArray());
    }

    [Fact]
    public void ShipsEmptyBeforeFleetUpdatedEvent()
    {
        Assert.Empty(_vm.Ships);
    }

    [Fact]
    public void SelectFleetAndDisplaysItsShips()
    {
        TriggerFleetUpdatedEvent([ShipId.From(1), ShipId.From(2), ShipId.From(3)], []);

        _state.Select(FleetId.From(1));

        Assert.Equal([ShipId.From(1), ShipId.From(2), ShipId.From(3)], _vm.Ships.Select(s => s.Id));
    }

    [Fact]
    public void UpdateShipsWhenFleetShipsChange()
    {
        TriggerFleetUpdatedEvent([ShipId.From(1), ShipId.From(2), ShipId.From(3)], []);
        _state.Select(FleetId.From(1));

        TriggerFleetUpdatedEvent([ShipId.From(1), ShipId.From(2), ShipId.From(4)], []);

        Assert.Equal([ShipId.From(1), ShipId.From(2), ShipId.From(4)], _vm.Ships.Select(s => s.Id));
    }

    [Fact]
    public void SwitchFleetAndUpdateShips()
    {
        TriggerFleetUpdatedEvent([ShipId.From(1), ShipId.From(2), ShipId.From(3)], [ShipId.From(5)]);

        _state.Select(FleetId.From(1));
        _state.Select(FleetId.From(2));

        Assert.Equal([ShipId.From(5)], _vm.Ships.Select(s => s.Id));
    }

    [Fact]
    public void EmptyFleetDisplaysNoShips()
    {
        var emptyFleet = Utils.GenerateMock<IFleetUpdated, FleetId>(3);
        emptyFleet.Ships.Returns([]);

        // Fleet 1 must exist because FleetSelectionState auto-selects it on first fleet update.
        _fleet1.Ships.Returns([ShipId.From(1), ShipId.From(2), ShipId.From(3)]);

        _fleetsUpdatedSubject.OnNext([_fleet1, emptyFleet]);
        _state.Select(FleetId.From(3));

        Assert.Empty(_vm.Ships);
    }

    private void TriggerFleetUpdatedEvent(IReadOnlyList<ShipId> fleet1Ships, IReadOnlyList<ShipId> fleet2Ships)
    {
        _fleet1.Ships.Returns(fleet1Ships);
        _fleet2.Ships.Returns(fleet2Ships);

        _fleetsUpdatedSubject.OnNext([_fleet1, _fleet2]);
    }
}
