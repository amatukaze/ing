using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public class AirForceSetPlane
    {
        public AirForceSetPlane(MapAreaId mapAreaId, AirForceGroupId groupId, int newDistance, IReadOnlyCollection<IRawAirForceSquadron> updatedSquadrons)
        {
            MapAreaId = mapAreaId;
            GroupId = groupId;
            NewDistance = newDistance;
            UpdatedSquadrons = updatedSquadrons;
        }

        public MapAreaId MapAreaId { get; }
        public AirForceGroupId GroupId { get; }
        public int NewDistance { get; }
        public IReadOnlyCollection<IRawAirForceSquadron> UpdatedSquadrons { get; }
    }
}
