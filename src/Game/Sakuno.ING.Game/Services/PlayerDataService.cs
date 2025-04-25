using System.Reactive.Linq;
using System.Reactive.Subjects;
using Injectio.Attributes;
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

        _materials = new(default);
        Materials = _materials.AsObservable();

        gameProvider.MaterialsUpdated.Scan(new Materials(), (materials, updated) =>
        {
            updated.Apply(ref materials);

            return materials;
        }).Subscribe(_materials);
    }
}
