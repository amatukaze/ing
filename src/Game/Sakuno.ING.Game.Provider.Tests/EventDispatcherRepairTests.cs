using NSubstitute;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Patches;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class EventDispatcherRepairTests
{
    [Fact]
    public void StartRepairWithoutInstantRepair()
    {
        var ship = CreateShip(1, currentHp: 15, maxHp: 30);
        var dock = CreateDock(1, shipId: ShipId.Empty);
        var (providerSource, dispatcher) = CreateDispatcher([ship], [dock]);

        dispatcher.HandleRepairStarted(RepairDockId.From(1), ShipId.From(1), useInstantRepair: false);

        providerSource.DidNotReceive().OnShipPatched(Arg.Any<IShipPatched>());
        providerSource.DidNotReceive().OnRepairDockPatched(Arg.Any<IRepairDockPatched>());
    }

    [Fact]
    public void StartRepairWithInstantRepairUsed()
    {
        var ship = CreateShip(1, currentHp: 15, maxHp: 30);
        var dock = CreateDock(1, shipId: ShipId.Empty);
        var (providerSource, dispatcher) = CreateDispatcher([ship], [dock]);

        dispatcher.HandleRepairStarted(RepairDockId.From(1), ShipId.From(1), useInstantRepair: true);

        providerSource.Received().OnShipPatched(Arg.Is<ShipInstantRepairPatch>(p =>
            p.Id == ShipId.From(1) && p.HP == new ShipHP(30, 30) && p.Morale == 40));
        providerSource.DidNotReceive().OnRepairDockPatched(Arg.Any<RepairDockInstantRepairPatch>());
    }

    [Fact]
    public void UseInstantRepair()
    {
        var ship = CreateShip(1, currentHp: 10, maxHp: 20);
        var dock = CreateDock(1, shipId: ShipId.From(1));
        var (providerSource, dispatcher) = CreateDispatcher([ship], [dock]);

        dispatcher.HandleInstantRepairUsed(RepairDockId.From(1));

        providerSource.Received().OnShipPatched(Arg.Is<ShipInstantRepairPatch>(p =>
            p.Id == ShipId.From(1) && p.HP == new ShipHP(20, 20) && p.Morale == 40));
        providerSource.Received().OnRepairDockPatched(Arg.Is<RepairDockInstantRepairPatch>(p =>
            p.Id == RepairDockId.From(1) &&
            p.State == RepairDockState.Idle &&
            p.ShipId == ShipId.Empty &&
            p.CompletionTime == DateTimeOffset.MinValue &&
            p.Consumption == default));
    }

    private static (IGameProviderSource ProviderSource, EventDispatcher Dispatcher) CreateDispatcher(Ship[] ships, RepairDock[] docks)
    {
        var shipTable = new TestTable<Ship, ShipId>(ships);
        var dockTable = new TestTable<RepairDock, RepairDockId>(docks);

        var providerSource = Substitute.For<IGameProviderSource>();
        var playerDataSnapshotService = Substitute.For<IPlayerDataSnapshotService>();
        playerDataSnapshotService.Ships.Returns(shipTable);
        playerDataSnapshotService.RepairDocks.Returns(dockTable);

        var apiMessageProvider = Substitute.For<IApiMessageProvider>();
        apiMessageProvider.ApiMessages.Returns(new System.Reactive.Subjects.Subject<ApiMessage>());

        var dispatcher = new EventDispatcher(providerSource, Substitute.For<IMasterDataSnapshotService>(), playerDataSnapshotService, apiMessageProvider);

        return (providerSource, dispatcher);
    }

    private static Ship CreateShip(int id, int currentHp, int maxHp)
    {
        var shipUpdate = Substitute.For<IShipUpdated>();
        shipUpdate.Id.Returns(ShipId.From(id));
        shipUpdate.MasterId.Returns(ShipInfoId.From(1));
        shipUpdate.HP.Returns(new ShipHP(currentHp, maxHp));

        return new Ship(shipUpdate);
    }

    private static RepairDock CreateDock(int id, ShipId shipId)
    {
        var dockUpdate = Substitute.For<IRepairDockUpdated>();
        dockUpdate.Id.Returns(RepairDockId.From(id));
        dockUpdate.State.Returns(shipId == ShipId.Empty ? RepairDockState.Idle : RepairDockState.Repairing);
        dockUpdate.ShipId.Returns(shipId);
        dockUpdate.Consumption.Returns(new Materials { Fuel = 10, Steel = 20 });

        return new RepairDock(dockUpdate);
    }
}
