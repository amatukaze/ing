using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawAirForceGroup
{
    AirForceGroupId IIdentifiable<AirForceGroupId>.Id => new(api_area_id, api_rid);
    int IAirForceGroupUpdated.BaseCombatRadius => api_distance.api_base;
    int IAirForceGroupUpdated.ExtraCombatRadius => api_distance.api_bonus;
}
