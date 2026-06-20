using System.Runtime.InteropServices;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Patches;
using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_req_hensei/change")]
    internal void HandleFleetOrganization([FromRequest("id")] FleetId fleetId,
        [FromRequest("ship_idx")] int targetShipIndex,
        [FromRequest("ship_id")] ShipId shipId)
    {
        Mutate(data =>
        {
            var targetFleetShipIds = GetSpan(data.Fleets[fleetId].Ships);
            ShipId[] resultShipIds;

            if (shipId == ShipId.From(-1))
            {
                resultShipIds = [..targetFleetShipIds[..targetShipIndex], ..targetFleetShipIds[(targetShipIndex + 1)..]];
                _provider.OnFleetPatched(new FleetShipsPatch(fleetId, resultShipIds));
                return;
            }

            if (shipId == ShipId.From(-2))
            {
                resultShipIds = [targetFleetShipIds[0]];
                _provider.OnFleetPatched(new FleetShipsPatch(fleetId, resultShipIds));
                return;
            }

            foreach (var fleet in data.Fleets)
            {
                var fleetShipIds = fleet.Id == fleetId ? targetFleetShipIds : GetSpan(fleet.Ships);
                var shipIndexInFleet = fleetShipIds.IndexOf(shipId);
                if (shipIndexInFleet is -1)
                    continue;

                if (fleet.Id == fleetId)
                {
                    resultShipIds = targetFleetShipIds.ToArray();
                    (resultShipIds[targetShipIndex], resultShipIds[shipIndexInFleet]) = (resultShipIds[shipIndexInFleet], resultShipIds[targetShipIndex]);

                    _provider.OnFleetPatched(new FleetShipsPatch(fleetId, resultShipIds));
                    return;
                }

                ShipId[] updatedFleetShipIds = targetShipIndex >= targetFleetShipIds.Length
                    ? [..fleetShipIds[..shipIndexInFleet], ..fleetShipIds[(shipIndexInFleet + 1)..]]
                    : [..fleetShipIds[..shipIndexInFleet], targetFleetShipIds[targetShipIndex], ..fleetShipIds[(shipIndexInFleet + 1)..]];

                _provider.OnFleetPatched(new FleetShipsPatch(fleet.Id, updatedFleetShipIds));
                break;
            }

            resultShipIds = targetFleetShipIds.Length switch
            {
                0 => [shipId],
                _ => targetShipIndex >= targetFleetShipIds.Length
                    ? [..targetFleetShipIds[..targetShipIndex], shipId]
                    : [..targetFleetShipIds[..targetShipIndex], shipId, ..targetFleetShipIds[(targetShipIndex + 1)..]]
            };

            _provider.OnFleetPatched(new FleetShipsPatch(fleetId, resultShipIds));
        });

        static ReadOnlySpan<T> GetSpan<T>(IReadOnlyList<T> list) =>
            list switch
            {
                T[] array => array.AsSpan(),
                List<T> concreteList => CollectionsMarshal.AsSpan(concreteList),
                _ => list.ToArray().AsSpan(),
            };
    }

    [Api("api_req_hensei/preset_select")]
    private void HandleFleetPresetApplied(RawFleet response) =>
        _provider.OnPartialFleetsUpdated([response]);
}
