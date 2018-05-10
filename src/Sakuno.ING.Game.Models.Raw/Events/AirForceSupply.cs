using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public readonly struct AirForceSupply
    {
        public AirForceSupply(int mapAreaId, int airForceId, IReadOnlyCollection<IRawAirForceSquadron> updatedSquadrons)
        {
            MapAreaId = mapAreaId;
            AirForceId = airForceId;
            UpdatedSquadrons = updatedSquadrons;
        }

        public int MapAreaId { get; }
        public int AirForceId { get; }
        public IReadOnlyCollection<IRawAirForceSquadron> UpdatedSquadrons { get; }
    }
}
