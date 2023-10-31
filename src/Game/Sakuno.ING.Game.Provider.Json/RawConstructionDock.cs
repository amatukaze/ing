using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawConstructionDock
{
    DateTimeOffset IConstructionDockUpdated.CompletionTime => DateTimeOffset.FromUnixTimeMilliseconds(api_complete_time);
    Materials IConstructionDockUpdated.Consumption => new()
    {
        Fuel = api_item1,
        Bullet = api_item2,
        Steel = api_item3,
        Bauxite = api_item4,
        Development = api_item5,
    };
}
