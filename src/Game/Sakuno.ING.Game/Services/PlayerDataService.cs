using System.Reactive.Linq;
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

    public IObservable<Admiral> Admiral { get; }

    public IObservable<Materials> Materials { get; }

    public PlayerDataService(IGameProvider gameProvider)
    {
        Ships = new Table<Ship, ShipId, IShipUpdated, IShipPatched>(gameProvider.ShipsUpdated,
            partialUpdateSource: gameProvider.PartialShipsUpdated,
            removeSource: gameProvider.ShipsRemoved,
            patchSource: gameProvider.ShipPatched,
            committingSource: gameProvider.Committed);

        SlotItems = new Table<SlotItem, SlotItemId, ISlotItemUpdated, ISlotItemPatched>(gameProvider.SlotItemsUpdated,
            partialUpdateSource: gameProvider.PartialSlotItemsUpdated,
            removeSource: gameProvider.SlotItemsRemoved,
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

        Admiral = new GameData<Admiral>(gameProvider.AdmiralIdUpdated.DistinctUntilChanged().Select(id =>
        {
            var initial = new Admiral(id);

            return gameProvider.AdmiralUpdated.Scan(initial, (admiral, updated) =>
            {
                admiral.Update(updated);

                return admiral;
            });
        }).Switch());

        Materials = new GameData<Materials>(gameProvider.MaterialsUpdated.Scan(new Materials(), (materials, updated) =>
        {
            updated.Apply(ref materials);

            return materials;
        }));
    }
}
