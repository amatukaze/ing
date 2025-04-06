using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IAirForceGroupUpdated : IIdentifiable<AirForceGroupId>
{
    string Name { get; }
    int BaseCombatRadius { get; }
    int ExtraCombatRadius { get; }
    AirForceGroupAction Action { get; }
    IReadOnlyList<IAirForceSquadronUpdated> Squadrons { get; }
}
