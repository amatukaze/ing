using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Patches;
using Sakuno.ING.Game.Provider.Json;

namespace Sakuno.ING.Game.Provider;

partial class EventDispatcher
{
    [Api("api_req_mission/return_instruction")]
    internal void HandleExpeditionRecalled([FromRequest("deck_id")] FleetId fleetId, ExpeditionRecallJson response)
    {
        if (response.api_mission is not { Length: >= 3 })
            return;

        _provider.OnFleetPatched(new FleetExpeditionPatch(
            fleetId,
            (ExpeditionId)response.api_mission[1],
            (FleetExpeditionState)response.api_mission[0],
            DateTimeOffset.FromUnixTimeMilliseconds(response.api_mission[2])));
    }
}
