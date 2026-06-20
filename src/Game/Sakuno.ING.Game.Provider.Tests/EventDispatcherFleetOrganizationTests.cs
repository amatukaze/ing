using System.Reactive.Subjects;
using NSubstitute;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class EventDispatcherFleetOrganizationTests
{
    [Fact]
    public void AppendShipAtEnd()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(2));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(2) })));
    }

    [Fact]
    public void AddShipToEmptyFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1)], []]);

        dispatcher.HandleFleetOrganization(FleetId.From(2), 1, ShipId.From(2));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(2) })));
    }

    [Fact]
    public void ReplaceShipAtMiddle()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(4));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(4), ShipId.From(3) })));
    }

    [Fact]
    public void RemoveFirstShip()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 0, ShipId.From(-1));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(2), ShipId.From(3) })));
    }

    [Fact]
    public void RemoveMiddleShip()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(-1));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(3) })));
    }

    [Fact]
    public void RemoveLastShip()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 2, ShipId.From(-1));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(2) })));
    }

    [Fact]
    public void ClearFleetExceptFlagship()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3), ShipId.From(4), ShipId.From(5)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 0, ShipId.From(-2));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1) })));
    }

    [Fact]
    public void SwapNonAdjacentShipsWithinFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3), ShipId.From(4), ShipId.From(5)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(5));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(5), ShipId.From(3), ShipId.From(4), ShipId.From(2) })));
    }

    [Fact]
    public void SwapAdjacentShipsWithinFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3), ShipId.From(4), ShipId.From(5)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 3, ShipId.From(3));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(2), ShipId.From(4), ShipId.From(3), ShipId.From(5) })));
    }

    [Fact]
    public void AppendShipFromOtherFleetAtHead()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1)], [ShipId.From(2), ShipId.From(3), ShipId.From(4)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(2));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(2) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(3), ShipId.From(4) })));
    }

    [Fact]
    public void AppendShipFromOtherFleetAtMiddle()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1)], [ShipId.From(2), ShipId.From(3), ShipId.From(4), ShipId.From(5)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(3));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(3) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(2), ShipId.From(4), ShipId.From(5) })));
    }

    [Fact]
    public void AppendShipFromOtherFleetAtTail()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1)], [ShipId.From(2), ShipId.From(3), ShipId.From(4)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(4));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(4) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(2), ShipId.From(3) })));
    }

    [Fact]
    public void ReplaceWithShipFromOtherFleetAtHead()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)], [ShipId.From(4), ShipId.From(5), ShipId.From(6)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 0, ShipId.From(4));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(4), ShipId.From(2), ShipId.From(3) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(5), ShipId.From(6) })));
    }

    [Fact]
    public void ReplaceWithShipFromOtherFleetAtMiddle()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)], [ShipId.From(4), ShipId.From(5), ShipId.From(6)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 1, ShipId.From(4));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(4), ShipId.From(3) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(2), ShipId.From(5), ShipId.From(6) })));
    }

    [Fact]
    public void ReplaceWithShipFromOtherFleetAtTail()
    {
        var (providerSource, dispatcher) = CreateDispatcher([[ShipId.From(1), ShipId.From(2), ShipId.From(3)], [ShipId.From(4), ShipId.From(5), ShipId.From(6)]]);

        dispatcher.HandleFleetOrganization(FleetId.From(1), 2, ShipId.From(6));

        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(1) && f.Ships.SequenceEqual(new[] { ShipId.From(1), ShipId.From(2), ShipId.From(6) })));
        providerSource.Received().OnFleetPatched(Arg.Is<FleetShipsPatch>(f =>
            f.Id == FleetId.From(2) && f.Ships.SequenceEqual(new[] { ShipId.From(4), ShipId.From(5), ShipId.From(3) })));
    }

    private static (IGameProviderSource ProviderSource, EventDispatcher Dispatcher) CreateDispatcher(ShipId[][] fleetShips)
    {
        var ships = new TestTable<Ship, ShipId>(Enumerable.Range(1, 12).Select(id => new Ship(ShipId.From(id))));
        var fleets = new TestTable<Fleet, FleetId>(fleetShips.Select((ships, index) =>
        {
            var fleet = Substitute.For<IFleetUpdated>();
            fleet.Id.Returns(FleetId.From(index + 1));
            fleet.Ships.Returns(ships);

            return new Fleet(fleet);
        }));

        var providerSource = Substitute.For<IGameProviderSource>();
        var playerDataSnapshotService = Substitute.For<IPlayerDataSnapshotService>();
        playerDataSnapshotService.Ships.Returns(ships);
        playerDataSnapshotService.Fleets.Returns(fleets);

        var apiMessageProvider = Substitute.For<IApiMessageProvider>();
        apiMessageProvider.ApiMessages.Returns(new Subject<ApiMessage>());

        var dispatcher = new EventDispatcher(providerSource, playerDataSnapshotService, apiMessageProvider);

        return (providerSource, dispatcher);
    }
}
