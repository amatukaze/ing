using System.Reactive.Linq;
using System.Reactive.Subjects;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Provider;

namespace Sakuno.ING.Game.Services;

[RegisterSingleton]
public class PlayerDataService
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

        Ships = new Table<Ship, ShipId, IShipUpdated>(gameProvider.ShipsUpdated, gameProvider.PartialShipsUpdated, removeShipsSubject);

        var removeSlotItemsSubject = new Subject<SlotItemId[]>();

        SlotItems = new Table<SlotItem, SlotItemId, ISlotItemUpdated>(gameProvider.SlotItemsUpdated, gameProvider.PartialSlotItemsUpdated, removeSlotItemsSubject);

        Fleets = new Table<Fleet, FleetId, IFleetUpdated>(gameProvider.FleetsUpdated, gameProvider.PartialFleetsUpdated);

        ConstructionDocks = new Table<ConstructionDock, ConstructionDockId, IConstructionDockUpdated>(gameProvider.ConstructionDocksUpdated);
        RepairDocks = new Table<RepairDock, RepairDockId, IRepairDockUpdated>(gameProvider.RepairDocksUpdated);

        UseItems = new Table<UseItem, UseItemId, IUseItemUpdated>(gameProvider.UseItemsUpdated);

        AirForceGroups = new Table<AirForceGroup, AirForceGroupId, IAirForceGroupUpdated>(gameProvider.AirForceGroupsUpdated);

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
