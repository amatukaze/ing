using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game;

public interface IPlayerDataService
{
    ITable<Ship, ShipId> Ships { get; }
    ITable<SlotItem, SlotItemId> SlotItems { get; }

    ITable<Fleet, FleetId> Fleets { get; }

    ITable<ConstructionDock, ConstructionDockId> ConstructionDocks { get; }
    ITable<RepairDock, RepairDockId> RepairDocks { get; }

    ITable<UseItem, UseItemId> UseItems { get; }

    IObservable<Admiral> Admiral { get; }

    IObservable<Materials> Materials { get; }
}
