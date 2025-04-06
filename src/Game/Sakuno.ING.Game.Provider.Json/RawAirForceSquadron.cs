namespace Sakuno.ING.Game.Provider.Json;

public partial class RawAirForceSquadron
{
    AirForceSquadronId IIdentifiable<AirForceSquadronId>.Id => api_squadron_id;
}
