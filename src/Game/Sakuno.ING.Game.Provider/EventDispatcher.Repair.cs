using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_req_nyukyo/start")]
    internal void HandleRepairStarted([FromRequest("ndock_id")] RepairDockId dockId,
        [FromRequest("ship_id")] ShipId shipId,
        [FromRequest("highspeed")] bool useInstantRepair)
    {
        if (!useInstantRepair)
            return;

        if (_playerDataSnapshotService.Ships.TryGetValue(shipId, out var ship))
            _provider.OnShipPatched(new ShipInstantRepairPatch(shipId, new ShipHP(ship.HP.Max, ship.HP.Max), 40));
    }

    [Api("api_req_nyukyo/speedchange")]
    internal void HandleInstantRepairUsed([FromRequest("ndock_id")] RepairDockId dockId)
    {
        if (!_playerDataSnapshotService.RepairDocks.TryGetValue(dockId, out var dock))
            return;

        var shipId = dock.ShipId;
        if (_playerDataSnapshotService.Ships.TryGetValue(shipId, out var ship))
            _provider.OnShipPatched(new ShipInstantRepairPatch(shipId, new ShipHP(ship.HP.Max, ship.HP.Max), 40));

        _provider.OnRepairDockPatched(new RepairDockInstantRepairPatch(
            dockId,
            RepairDockState.Idle,
            DateTimeOffset.MinValue,
            default,
            ShipId.Empty));
    }
}
