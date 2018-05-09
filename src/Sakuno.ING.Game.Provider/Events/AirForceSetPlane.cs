using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public class AirForceSetPlane
    {
        public int MapAreaId { get; internal set; }
        public int AirForceId { get; internal set; }
        public int NewDistance { get; internal set; }
        public IReadOnlyCollection<IRawAirForceSquadron> UpdatedSquadrons { get; internal set; }
    }
}
