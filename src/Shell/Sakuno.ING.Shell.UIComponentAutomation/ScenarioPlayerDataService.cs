using System.Reactive.Linq;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models.MasterData;
using GameMaterials = Sakuno.ING.Game.Models.Materials;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal sealed class ScenarioPlayerDataService : IPlayerDataService
{
    public ITable<Ship, ShipId> Ships => null!;
    public ITable<SlotItem, SlotItemId> SlotItems => null!;
    public ITable<Fleet, FleetId> Fleets => null!;
    public ITable<ConstructionDock, ConstructionDockId> ConstructionDocks => null!;
    public ITable<RepairDock, RepairDockId> RepairDocks => null!;
    public ITable<UseItem, UseItemId> UseItems => null!;
    public IObservable<Admiral> Admiral { get; } = Observable.Empty<Admiral>();

    public IObservable<GameMaterials> Materials { get; } = Observable.Return(new GameMaterials
    {
        Fuel = 1101,
        Bullet = 1202,
        Steel = 1303,
        Bauxite = 1404,
        InstantBuild = 15,
        InstantRepair = 26,
        Development = 37,
        Improvement = 48,
    });
}
