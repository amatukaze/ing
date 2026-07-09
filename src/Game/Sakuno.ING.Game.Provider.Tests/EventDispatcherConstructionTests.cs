using System.Reactive.Subjects;
using NSubstitute;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Patches;
using Sakuno.ING.Game.Provider.Json;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class EventDispatcherConstructionTests
{
    [Fact]
    public void InstantBuildUsed()
    {
        var (providerSource, dispatcher) = CreateDispatcher([CreateDock(1, ConstructionDockState.Building)]);

        dispatcher.HandleInstantBuildUsed(ConstructionDockId.From(1));

        providerSource.Received().OnConstructionDockPatched(Arg.Is<ConstructionDockCompletedPatch>(p =>
            p.Id == ConstructionDockId.From(1) &&
            p.State == ConstructionDockState.Completed &&
            p.CompletionTime == DateTimeOffset.MinValue));
    }

    [Fact]
    public void GetShipUpdatesDockShipAndSlotItems()
    {
        var (providerSource, dispatcher) = CreateDispatcher([]);

        dispatcher.HandleShipBuilt(new GetShipJson
        {
            api_kdock = [CreateDockRaw(1, ConstructionDockState.Idle)],
            api_ship = CreateShipRaw(1),
            api_slotitem = [CreateSlotItemRaw(101)],
        });

        providerSource.Received().OnConstructionDocksUpdated(Arg.Is<IReadOnlyList<IConstructionDockUpdated>>(list =>
            list.Count == 1 && list[0].Id == ConstructionDockId.From(1)));
        providerSource.Received().OnPartialShipsUpdated(Arg.Is<IReadOnlyList<IShipUpdated>>(list =>
            list.Count == 1 && list[0].Id == ShipId.From(1)));
        providerSource.Received().OnPartialSlotItemsUpdated(Arg.Is<IReadOnlyList<ISlotItemUpdated>>(list =>
            list.Count == 1 && list[0].Id == SlotItemId.From(101)));
    }

    private static (IGameProviderSource ProviderSource, EventDispatcher Dispatcher) CreateDispatcher(ConstructionDock[] docks)
    {
        var dockTable = new TestTable<ConstructionDock, ConstructionDockId>(docks);

        var providerSource = Substitute.For<IGameProviderSource>();
        var playerDataSnapshotService = Substitute.For<IPlayerDataSnapshotService>();
        playerDataSnapshotService.ConstructionDocks.Returns(dockTable);

        var apiMessageProvider = Substitute.For<IApiMessageProvider>();
        apiMessageProvider.ApiMessages.Returns(new Subject<ApiMessage>());

        var dispatcher = new EventDispatcher(providerSource, Substitute.For<IMasterDataSnapshotService>(), playerDataSnapshotService, apiMessageProvider);

        return (providerSource, dispatcher);
    }

    private static ConstructionDock CreateDock(int id, ConstructionDockState state)
    {
        var dockUpdate = Substitute.For<IConstructionDockUpdated>();
        dockUpdate.Id.Returns(ConstructionDockId.From(id));
        dockUpdate.State.Returns(state);
        dockUpdate.Consumption.Returns(new Materials { Fuel = 10, Bullet = 20, Steel = 30, Bauxite = 40 });
        dockUpdate.ResultShipId.Returns(ShipInfoId.From(1));

        return new ConstructionDock(dockUpdate);
    }

    private static RawConstructionDock CreateDockRaw(int id, ConstructionDockState state) =>
        new()
        {
            api_id = ConstructionDockId.From(id),
            api_state = state,
            api_item1 = 0,
            api_item2 = 0,
            api_item3 = 0,
            api_item4 = 0,
            api_item5 = 0,
            api_complete_time = 0,
            api_created_ship_id = ShipInfoId.Empty,
        };

    private static RawShip CreateShipRaw(int id) =>
        new()
        {
            api_id = ShipId.From(id),
            api_ship_id = ShipInfoId.From(1),
        };

    private static RawSlotItem CreateSlotItemRaw(int id) =>
        new()
        {
            api_id = SlotItemId.From(id),
            api_slotitem_id = SlotItemInfoId.From(1),
        };
}
