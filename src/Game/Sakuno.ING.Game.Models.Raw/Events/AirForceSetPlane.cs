using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public class AirForceSetPlane
    {
        public AirForceSetPlane(MapAreaId mapAreaId, AirForceGroupId groupId, int newDistanceBase, int newDistanceBonus, IReadOnlyCollection<IRawAirForceSquadron> updatedSquadrons)
        {
            MapAreaId = mapAreaId;
            GroupId = groupId;
            NewDistanceBase = newDistanceBase;
            NewDistanceBonus = newDistanceBonus;
            UpdatedSquadrons = updatedSquadrons;
        }

        public MapAreaId MapAreaId { get; }
        public AirForceGroupId GroupId { get; }
        public int NewDistanceBase { get; }
        public int NewDistanceBonus { get; }
        public IReadOnlyCollection<IRawAirForceSquadron> UpdatedSquadrons { get; }
    }
}
