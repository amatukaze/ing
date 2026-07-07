using System.Runtime.InteropServices;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_req_kousyou/destroyship")]
    internal void HandleShipDestroyed([FromRequest("ship_id")] ShipId[] shipIds, [FromRequest("slot_dest_flag")] bool removeSlotItems)
    {
        _provider.OnShipsRemoved(shipIds);

        Dictionary<FleetId, List<ShipId>>? fleetPatches = null;

        foreach (var shipId in shipIds)
            foreach (var fleet in _playerDataSnapshotService.Fleets)
            {
                var patchingShipIds = fleetPatches?.GetValueOrDefault(fleet.Id);
                var shipIndexInFleet = GetSpan(patchingShipIds ?? fleet.Ships).IndexOf(shipId);
                if (shipIndexInFleet is -1)
                    continue;

                if (patchingShipIds is null)
                {
                    patchingShipIds = fleet.Ships.ToList();

                    fleetPatches ??= new(_playerDataSnapshotService.Fleets.Count);
                    fleetPatches[fleet.Id] = patchingShipIds;
                }

                patchingShipIds.RemoveAt(shipIndexInFleet);
                break;
            }

        if (fleetPatches is not null)
            foreach (var (fleetId, updatedFleetShipIds) in fleetPatches)
                _provider.OnFleetPatched(new FleetShipsPatch(fleetId, updatedFleetShipIds));

        if (!removeSlotItems)
            return;

        foreach (var shipId in shipIds)
        {
            if (!_playerDataSnapshotService.Ships.TryGetValue(shipId, out var ship))
                continue;

            _provider.OnSlotItemsRemoved(ship.SlotItems.Append(ship.ExtraSlot).Where(id => id.IsValid).ToArray());
        }

        static ReadOnlySpan<T> GetSpan<T>(IReadOnlyList<T> list) =>
            list switch
            {
                T[] array => array.AsSpan(),
                List<T> concreteList => CollectionsMarshal.AsSpan(concreteList),
                _ => list.ToArray().AsSpan(),
            };
    }

    [Api("api_req_kousyou/destroyitem2")]
    internal void HandleSlotItemsDestroyed([FromRequest("slotitem_ids")] SlotItemId[] slotItemIds)
    {
        _provider.OnSlotItemsRemoved(slotItemIds);
    }
}
