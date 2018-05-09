using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public struct AirForceSupply
    {
        public int MapAreaId;
        public int AirForceId;
        public IReadOnlyCollection<IRawAirForceSquadron> UpdatedSquadrons;
    }
}
