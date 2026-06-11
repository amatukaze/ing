using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Services;

[RegisterSingleton(Registration = RegistrationStrategy.SelfWithProxyFactory)]
internal class PlayerDataSnapshotService(PlayerDataService service) : IPlayerDataSnapshotService
{
    public ITableSnapshot<Ship, ShipId> Ships => service.Ships.Snapshot;
    public ITableSnapshot<SlotItem, SlotItemId> SlotItems => service.SlotItems.Snapshot;
    public ITableSnapshot<Fleet, FleetId> Fleets => service.Fleets.Snapshot;
    public ITableSnapshot<ConstructionDock, ConstructionDockId> ConstructionDocks => service.ConstructionDocks.Snapshot;
    public ITableSnapshot<RepairDock, RepairDockId> RepairDocks => service.RepairDocks.Snapshot;
    public ITableSnapshot<UseItem, UseItemId> UseItems => service.UseItems.Snapshot;
}
