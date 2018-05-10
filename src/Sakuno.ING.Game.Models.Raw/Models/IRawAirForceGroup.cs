using System.Collections.Generic;

namespace Sakuno.ING.Game.Models
{
    public interface IRawAirForceGroup : IIdentifiable
    {
        int MapAreaId { get; }
        int GroupId { get; }
        string Name { get; }
        int Distance { get; }
        AirForceAction Action { get; }
        IReadOnlyCollection<IRawAirForceSquadron> Squadrons { get; }
    }

    public enum AirForceAction
    {
        Standby = 0,
        Sortie = 1,
        Defense = 2,
        Retreat = 3,
        Rest = 4
    }
}
