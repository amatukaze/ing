using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
    public sealed class AirForceSquadronDeployment
    {
        public MapAreaId MapAreaId { get; }
        public AirForceGroupId GroupId { get; }
        public int BaseCombatRadius { get; }
        public int BonusCombatRadius { get; }
        public IReadOnlyCollection<RawAirForceSquadron> Squadrons { get; }

        public AirForceSquadronDeployment(MapAreaId mapAreaId, AirForceGroupId groupId, int baseCombatRadius, int bonusCombatRadius, IReadOnlyCollection<RawAirForceSquadron> updatedSquadrons)
        {
            MapAreaId = mapAreaId;
            GroupId = groupId;
            BaseCombatRadius = baseCombatRadius;
            BonusCombatRadius = bonusCombatRadius;
            Squadrons = updatedSquadrons;
        }
    }
}
