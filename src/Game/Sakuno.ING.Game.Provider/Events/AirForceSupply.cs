using System.Collections.Generic;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events
{
    public readonly struct AirForceSupply
    {
        public AirForceSupply(MapAreaId mapAreaId, AirForceGroupId groupId, IReadOnlyCollection<RawAirForceSquadron> updatedSquadrons)
        {
            MapAreaId = mapAreaId;
            GroupId = groupId;
            UpdatedSquadrons = updatedSquadrons;
        }

        public MapAreaId MapAreaId { get; }
        public AirForceGroupId GroupId { get; }
        public IReadOnlyCollection<RawAirForceSquadron> UpdatedSquadrons { get; }
    }
}
