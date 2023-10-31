using Sakuno.ING.Game.Events.MasterData;

namespace Sakuno.ING.Game.Provider.Json.MasterData;

public partial class RawShipInfo
{
    Materials IShipInfoUpdated.RemodelConsumption => new()
    {
        Fuel = api_afterfuel,
        Bullet = api_afterbull,
    };

    TimeSpan IShipInfoUpdated.ConstructionTime => TimeSpan.FromMinutes(api_buildtime);
}
