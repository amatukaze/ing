using NSubstitute;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Patches;
using Sakuno.ING.Game.Provider.Json;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class EventDispatcherExpeditionTests
{
    [Fact]
    public void ExpeditionRecalledPatchesFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher([CreateFleet(2)]);

        dispatcher.HandleExpeditionRecalled(
            FleetId.From(2),
            new ExpeditionRecallJson
            {
                api_mission = [(long)FleetExpeditionState.Recalled, 37, 1_700_000_000_000, 0],
            });

        providerSource.Received().OnFleetPatched(Arg.Is<FleetExpeditionPatch>(p =>
            p.Id == FleetId.From(2) &&
            p.ExpeditionState == FleetExpeditionState.Recalled &&
            p.ExpeditionId == ExpeditionId.From(37) &&
            p.ExpeditionCompletionTime == DateTimeOffset.FromUnixTimeMilliseconds(1_700_000_000_000)));
    }

    [Fact]
    public void ExpeditionRecalledWithoutMissionDataDoesNotPatchFleet()
    {
        var (providerSource, dispatcher) = CreateDispatcher([CreateFleet(2)]);

        dispatcher.HandleExpeditionRecalled(
            FleetId.From(2),
            new ExpeditionRecallJson());

        providerSource.DidNotReceive().OnFleetPatched(Arg.Any<IFleetPatched>());
    }

    private static (IGameProviderSource ProviderSource, EventDispatcher Dispatcher) CreateDispatcher(Fleet[] fleets)
    {
        var fleetTable = new TestTable<Fleet, FleetId>(fleets);

        var providerSource = Substitute.For<IGameProviderSource>();
        var playerDataSnapshotService = Substitute.For<IPlayerDataSnapshotService>();
        playerDataSnapshotService.Fleets.Returns(fleetTable);

        var apiMessageProvider = Substitute.For<IApiMessageProvider>();
        apiMessageProvider.ApiMessages.Returns(new System.Reactive.Subjects.Subject<ApiMessage>());

        var dispatcher = new EventDispatcher(providerSource, Substitute.For<IMasterDataSnapshotService>(), playerDataSnapshotService, apiMessageProvider);

        return (providerSource, dispatcher);
    }

    private static Fleet CreateFleet(int id)
    {
        var fleetUpdate = Substitute.For<IFleetUpdated>();
        fleetUpdate.Id.Returns(FleetId.From(id));
        fleetUpdate.Ships.Returns([ShipId.From(id * 10 + 1), ShipId.From(id * 10 + 2)]);
        fleetUpdate.ExpeditionState.Returns(FleetExpeditionState.None);

        return new Fleet(fleetUpdate);
    }
}
