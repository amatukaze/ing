using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game;

public interface IPlayerDataSnapshotService
{
    ITableSnapshot<Ship, ShipId> Ships { get; }
    ITableSnapshot<SlotItem, SlotItemId> SlotItems { get; }

    ITableSnapshot<Fleet, FleetId> Fleets { get; }

    ITableSnapshot<ConstructionDock, ConstructionDockId> ConstructionDocks { get; }
    ITableSnapshot<RepairDock, RepairDockId> RepairDocks { get; }

    ITableSnapshot<UseItem, UseItemId> UseItems { get; }
}
