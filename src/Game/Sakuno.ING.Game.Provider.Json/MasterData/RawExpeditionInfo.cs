using Sakuno.ING.Game.Events.MasterData;

namespace Sakuno.ING.Game.Provider.Json.MasterData;

public partial class RawExpeditionInfo
{
    TimeSpan IExpeditionInfoUpdated.Duration => TimeSpan.FromMinutes(api_time);
}
