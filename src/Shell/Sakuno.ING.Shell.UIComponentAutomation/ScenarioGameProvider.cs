using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.MasterData;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Provider;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal sealed class ScenarioGameProvider : IGameProvider
{
    private readonly BehaviorSubject<IReadOnlyList<IFleetUpdated>> _fleetsUpdated;

    public IReadOnlyList<FleetId> PublishedFleetIds { get; }

    public ScenarioGameProvider()
    {
        var fleets = Enumerable.Range(1, 3)
            .Select(id => (IFleetUpdated)new ScenarioFleetUpdated(FleetId.From(id)))
            .ToArray();
        PublishedFleetIds = fleets.Select(fleet => fleet.Id).ToArray();
        _fleetsUpdated = new(fleets);
    }

    public IObservable<IReadOnlyList<IFleetUpdated>> FleetsUpdated => _fleetsUpdated;

    public IObservable<Unit> Committed { get; } = Observable.Empty<Unit>();
    public IObservable<IMasterDataUpdated> MasterDataUpdated { get; } = Observable.Empty<IMasterDataUpdated>();
    public IObservable<AdmiralId> AdmiralIdUpdated { get; } = Observable.Empty<AdmiralId>();
    public IObservable<IAdmiralUpdated> AdmiralUpdated { get; } = Observable.Empty<IAdmiralUpdated>();
    public IObservable<IMaterialsUpdated> MaterialsUpdated { get; } = Observable.Empty<IMaterialsUpdated>();
    public IObservable<IReadOnlyList<IShipUpdated>> ShipsUpdated { get; } = Observable.Empty<IReadOnlyList<IShipUpdated>>();
    public IObservable<IReadOnlyList<IShipUpdated>> PartialShipsUpdated { get; } = Observable.Empty<IReadOnlyList<IShipUpdated>>();
    public IObservable<IShipPatched> ShipPatched { get; } = Observable.Empty<IShipPatched>();
    public IObservable<IReadOnlyList<ShipId>> ShipsRemoved { get; } = Observable.Empty<IReadOnlyList<ShipId>>();
    public IObservable<IReadOnlyList<ISlotItemUpdated>> SlotItemsUpdated { get; } = Observable.Empty<IReadOnlyList<ISlotItemUpdated>>();
    public IObservable<IReadOnlyList<ISlotItemUpdated>> PartialSlotItemsUpdated { get; } = Observable.Empty<IReadOnlyList<ISlotItemUpdated>>();
    public IObservable<ISlotItemPatched> SlotItemPatched { get; } = Observable.Empty<ISlotItemPatched>();
    public IObservable<IReadOnlyList<SlotItemId>> SlotItemsRemoved { get; } = Observable.Empty<IReadOnlyList<SlotItemId>>();
    public IObservable<IReadOnlyList<IFleetUpdated>> PartialFleetsUpdated { get; } = Observable.Empty<IReadOnlyList<IFleetUpdated>>();
    public IObservable<IFleetPatched> FleetPatched { get; } = Observable.Empty<IFleetPatched>();
    public IObservable<IReadOnlyList<IConstructionDockUpdated>> ConstructionDocksUpdated { get; } = Observable.Empty<IReadOnlyList<IConstructionDockUpdated>>();
    public IObservable<IConstructionDockPatched> ConstructionDockPatched { get; } = Observable.Empty<IConstructionDockPatched>();
    public IObservable<IReadOnlyList<IRepairDockUpdated>> RepairDocksUpdated { get; } = Observable.Empty<IReadOnlyList<IRepairDockUpdated>>();
    public IObservable<IRepairDockPatched> RepairDockPatched { get; } = Observable.Empty<IRepairDockPatched>();
    public IObservable<IReadOnlyList<IUseItemUpdated>> UseItemsUpdated { get; } = Observable.Empty<IReadOnlyList<IUseItemUpdated>>();
    public IObservable<IReadOnlyList<IUnequippedSlotItemsUpdated>> UnequippedSlotItemsUpdated { get; } = Observable.Empty<IReadOnlyList<IUnequippedSlotItemsUpdated>>();
    public IObservable<IReadOnlyList<IAirForceGroupUpdated>> AirForceGroupsUpdated { get; } = Observable.Empty<IReadOnlyList<IAirForceGroupUpdated>>();
    public IObservable<IAirForceGroupPatched> AirForceGroupPatched { get; } = Observable.Empty<IAirForceGroupPatched>();
    public IObservable<IReadOnlyList<IQuestUpdated>> PartialQuestsUpdated { get; } = Observable.Empty<IReadOnlyList<IQuestUpdated>>();

    private sealed record ScenarioFleetUpdated(FleetId Id) : IFleetUpdated
    {
        public string Name => $"Fleet {Id}";
        public IReadOnlyList<ShipId> Ships => [];
        public FleetExpeditionState ExpeditionState => default;
        public ExpeditionId ExpeditionId => default;
        public DateTimeOffset ExpeditionCompletionTime => default;
    }
}
