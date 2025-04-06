using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IAirForceSquadronUpdated : IIdentifiable<AirForceSquadronId>
{
    SlotItemId SlotItemId { get; }
    int Count { get; }
    int Capacity { get; }
    AirForceSquadronMorale Morale { get; }
}
