using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IExpeditionInfoUpdated : IIdentifiable<ExpeditionId>
{
    string DisplayId { get; }
    string Name { get; }
    MapAreaId MapAreaId { get; }

    TimeSpan Duration { get; }

    double FuelConsumptionPercentage { get; }
    double BulletConsumptionPercentage { get; }
}
