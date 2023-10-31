using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public partial class RawRepairDock
{
    DateTimeOffset IRepairDockUpdated.CompletionTime => DateTimeOffset.FromUnixTimeMilliseconds(api_complete_time);
    Materials IRepairDockUpdated.Consumption => new()
    {
        Fuel = api_item1,
        Steel = api_item3,
    };
}
