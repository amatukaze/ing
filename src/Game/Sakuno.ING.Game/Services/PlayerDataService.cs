using System.Reactive.Linq;
using System.Reactive.Subjects;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Provider;

namespace Sakuno.ING.Game.Services;

[RegisterSingleton(Registration = RegistrationStrategy.ImplementedInterfaces)]
internal class PlayerDataService : IPlayerDataService
{
    public ITable<Ship, ShipId> Ships { get; }
    public ITable<SlotItem, SlotItemId> SlotItems { get; }

    public ITable<Fleet, FleetId> Fleets { get; }

    public ITable<ConstructionDock, ConstructionDockId> ConstructionDocks { get; }
    public ITable<RepairDock, RepairDockId> RepairDocks { get; }

    public ITable<UseItem, UseItemId> UseItems { get; }

    public ITable<AirForceGroup, AirForceGroupId> AirForceGroups { get; }

    private readonly BehaviorSubject<Admiral?> _admiral;
    public IObservable<Admiral> Admiral { get; }
    public Admiral AdmiralSnapshot => _admiral.Value ?? throw new InvalidOperationException("Game not initialized");

    private readonly BehaviorSubject<Materials> _materials;
    public IObservable<Materials> Materials { get; }
    public Materials MaterialsSnapshot => _materials.Value;

    public PlayerDataService(IGameProvider gameProvider)
    {
        var removeShipsSubject = new Subject<ShipId[]>();

        Ships = new Table<Ship, ShipId, IShipUpdated, IShipPatched>(gameProvider.ShipsUpdated,
            partialUpdateSource: gameProvider.PartialShipsUpdated,
            removeSource: removeShipsSubject,
            patchSource: gameProvider.ShipPatched,
            committingSource: gameProvider.Committed);

        var removeSlotItemsSubject = new Subject<SlotItemId[]>();

        SlotItems = new Table<SlotItem, SlotItemId, ISlotItemUpdated, ISlotItemPatched>(gameProvider.SlotItemsUpdated,
            partialUpdateSource: gameProvider.PartialSlotItemsUpdated,
            removeSource: removeSlotItemsSubject,
            patchSource: gameProvider.SlotItemPatched,
            committingSource: gameProvider.Committed);

        Fleets = new Table<Fleet, FleetId, IFleetUpdated, IFleetPatched>(gameProvider.FleetsUpdated,
            partialUpdateSource: gameProvider.PartialFleetsUpdated,
            patchSource: gameProvider.FleetPatched,
            committingSource: gameProvider.Committed);

        ConstructionDocks = new Table<ConstructionDock, ConstructionDockId, IConstructionDockUpdated, IConstructionDockPatched>(gameProvider.ConstructionDocksUpdated,
            patchSource: gameProvider.ConstructionDockPatched,
            committingSource: gameProvider.Committed);
        RepairDocks = new Table<RepairDock, RepairDockId, IRepairDockUpdated, IRepairDockPatched>(gameProvider.RepairDocksUpdated,
            patchSource: gameProvider.RepairDockPatched,
            committingSource: gameProvider.Committed);

        UseItems = new Table<UseItem, UseItemId, IUseItemUpdated>(gameProvider.UseItemsUpdated);

        AirForceGroups = new Table<AirForceGroup, AirForceGroupId, IAirForceGroupUpdated, IAirForceGroupPatched>(gameProvider.AirForceGroupsUpdated,
            patchSource: gameProvider.AirForceGroupPatched,
            committingSource: gameProvider.Committed);

        _admiral = new(null);
        Admiral = _admiral.Where(m => m is not null).AsObservable()!;

        gameProvider.AdmiralUpdated.Scan((Admiral?)null, (admiral, raw) =>
        {
            if (admiral is null)
                return new Admiral(raw);

            admiral.Update(raw);
            return admiral;
        }).Subscribe(_admiral);

        _materials = new(default);
        Materials = _materials.AsObservable();

        gameProvider.MaterialsUpdated.Scan(new Materials(), (materials, updated) =>
        {
            updated.Apply(ref materials);

            return materials;
        }).Subscribe(_materials);
    }
}
