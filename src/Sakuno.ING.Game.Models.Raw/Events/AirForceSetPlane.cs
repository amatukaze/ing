using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public class AirForceSetPlane
    {
        public AirForceSetPlane(int mapAreaId, int airForceId, int newDistance, IReadOnlyCollection<IRawAirForceSquadron> updatedSquadrons)
        {
            MapAreaId = mapAreaId;
            AirForceId = airForceId;
            NewDistance = newDistance;
            UpdatedSquadrons = updatedSquadrons;
        }

        public int MapAreaId { get; }
        public int AirForceId { get; }
        public int NewDistance { get; }
        public IReadOnlyCollection<IRawAirForceSquadron> UpdatedSquadrons { get; }
    }
}
