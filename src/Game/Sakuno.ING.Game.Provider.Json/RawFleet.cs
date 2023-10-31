using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawFleet
{
    FleetExpeditionState IFleetUpdated.ExpeditionState => (FleetExpeditionState)api_mission[0];
    ExpeditionId IFleetUpdated.ExpeditionId => (ExpeditionId)api_mission[1];
    DateTimeOffset IFleetUpdated.ExpeditionCompletionTime => DateTimeOffset.FromUnixTimeMilliseconds(api_mission[2]);
}
