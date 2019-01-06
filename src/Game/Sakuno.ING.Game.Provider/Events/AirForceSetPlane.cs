using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public sealed class AirForceSetPlane
    {
        public AirForceSetPlane(MapAreaId mapAreaId, AirForceGroupId groupId, int newDistanceBase, int newDistanceBonus, IReadOnlyCollection<RawAirForceSquadron> updatedSquadrons)
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
        public IReadOnlyCollection<RawAirForceSquadron> UpdatedSquadrons { get; }
    }
}
