using NSubstitute;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class EventDispatcherArsenalTests
{
    [Fact]
    public void DestroyShipNotInFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: [CreateShip(1)],
            fleetShips: [[]]);

        dispatcher.HandleShipDestroyed([ShipId.From(1)], removeSlotItems: false);

        providerSource.Received().OnShipsRemoved(Arg.Is<IReadOnlyList<ShipId>>(ids =>
            ids.SequenceEqual(new[] { ShipId.From(1) })));
        providerSource.DidNotReceive().OnFleetPatched(Arg.Any<IFleetPatched>());
        providerSource.DidNotReceive().OnSlotItemsRemoved(Arg.Any<IReadOnlyList<SlotItemId>>());
    }

    [Fact]
    public void DestroyShipInFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: [CreateShip(1), CreateShip(2)],
            fleetShips: [[ShipId.From(1), ShipId.From(2)]]);

        dispatcher.HandleShipDestroyed([ShipId.From(2)], removeSlotItems: false);

        providerSource.Received().OnShipsRemoved(Arg.Is<IReadOnlyList<ShipId>>(ids =>
            ids.SequenceEqual(new[] { ShipId.From(2) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1) })));
        providerSource.DidNotReceive().OnSlotItemsRemoved(Arg.Any<IReadOnlyList<SlotItemId>>());
    }

    [Fact]
    public void DestroyShipsAcrossMultipleFleets()
    {
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: Enumerable.Range(1, 4).Select(id => CreateShip(id)).ToArray(),
            fleetShips:
            [
                [ShipId.From(1), ShipId.From(2)],
                [ShipId.From(3), ShipId.From(4)],
            ]);

        dispatcher.HandleShipDestroyed([ShipId.From(2), ShipId.From(3)], removeSlotItems: false);

        providerSource.Received().OnShipsRemoved(Arg.Is<IReadOnlyList<ShipId>>(ids =>
            ids.SequenceEqual(new[] { ShipId.From(2), ShipId.From(3) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(4) })));
    }

    [Fact]
    public void DestroyMultipleShipsInSameFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: Enumerable.Range(1, 5).Select(id => CreateShip(id)).ToArray(),
            fleetShips:
            [
                [ShipId.From(1), ShipId.From(2), ShipId.From(3), ShipId.From(4), ShipId.From(5)],
            ]);

        dispatcher.HandleShipDestroyed([ShipId.From(2), ShipId.From(4)], removeSlotItems: false);

        providerSource.Received().OnShipsRemoved(Arg.Is<IReadOnlyList<ShipId>>(ids =>
            ids.SequenceEqual(new[] { ShipId.From(2), ShipId.From(4) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(3), ShipId.From(5) })));
    }

    [Fact]
    public void DestroyShipWithSlotItems()
    {
        var ship = CreateShip(1, slotItems: [SlotItemId.From(101), SlotItemId.From(102)], extraSlot: SlotItemId.From(103));
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: [ship],
            fleetShips: [[ShipId.From(1)]]);

        dispatcher.HandleShipDestroyed([ShipId.From(1)], removeSlotItems: true);

        providerSource.Received().OnShipsRemoved(Arg.Is<IReadOnlyList<ShipId>>(ids =>
            ids.SequenceEqual(new[] { ShipId.From(1) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(Array.Empty<ShipId>())));
        providerSource.Received().OnSlotItemsRemoved(Arg.Is<IReadOnlyList<SlotItemId>>(ids =>
            ids.SequenceEqual(new[] { SlotItemId.From(101), SlotItemId.From(102), SlotItemId.From(103) })));
    }

    [Fact]
    public void DestroyShipWithoutRemovingSlotItems()
    {
        var ship = CreateShip(1, slotItems: [SlotItemId.From(101)], extraSlot: SlotItemId.From(102));
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: [ship],
            fleetShips: [[ShipId.From(1)]]);

        dispatcher.HandleShipDestroyed([ShipId.From(1)], removeSlotItems: false);

        providerSource.Received().OnShipsRemoved(Arg.Any<IReadOnlyList<ShipId>>());
        providerSource.Received().OnFleetPatched(Arg.Any<IFleetPatched>());
        providerSource.DidNotReceive().OnSlotItemsRemoved(Arg.Any<IReadOnlyList<SlotItemId>>());
    }

    [Fact]
    public void DestroySlotItems()
    {
        var (providerSource, dispatcher) = CreateDispatcher(
            ships: [CreateShip(1)],
            fleetShips: [[]]);

        dispatcher.HandleSlotItemsDestroyed([SlotItemId.From(101), SlotItemId.From(102)]);

        providerSource.Received().OnSlotItemsRemoved(Arg.Is<IReadOnlyList<SlotItemId>>(ids =>
            ids.SequenceEqual(new[] { SlotItemId.From(101), SlotItemId.From(102) })));
    }

    private static (IGameProviderSource ProviderSource, EventDispatcher Dispatcher) CreateDispatcher(Ship[] ships, ShipId[][] fleetShips)
    {
        var shipTable = new TestTable<Ship, ShipId>(ships);
        var fleetTable = new TestTable<Fleet, FleetId>(fleetShips.Select((shipIds, index) =>
        {
            var fleet = Substitute.For<IFleetUpdated>();
            fleet.Id.Returns(FleetId.From(index + 1));
            fleet.Ships.Returns(shipIds);

            return new Fleet(fleet);
        }));

        var providerSource = Substitute.For<IGameProviderSource>();
        var playerDataSnapshotService = Substitute.For<IPlayerDataSnapshotService>();
        playerDataSnapshotService.Ships.Returns(shipTable);
        playerDataSnapshotService.Fleets.Returns(fleetTable);

        var apiMessageProvider = Substitute.For<IApiMessageProvider>();
        apiMessageProvider.ApiMessages.Returns(new System.Reactive.Subjects.Subject<ApiMessage>());

        var dispatcher = new EventDispatcher(providerSource, Substitute.For<IMasterDataSnapshotService>(), playerDataSnapshotService, apiMessageProvider);

        return (providerSource, dispatcher);
    }

    private static Ship CreateShip(int id, SlotItemId[]? slotItems = null, SlotItemId extraSlot = default)
    {
        var shipUpdate = Substitute.For<IShipUpdated>();
        shipUpdate.Id.Returns(ShipId.From(id));
        shipUpdate.SlotItems.Returns(slotItems ?? Array.Empty<SlotItemId>());
        shipUpdate.ExtraSlot.Returns(extraSlot);

        return new Ship(shipUpdate);
    }
}
