using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public abstract class BattleStage : ModelBase
    {
        public BattleInfo Owner { get; }

        public abstract BattleStageType Type { get; }

        public abstract IList<BattlePhase> Phases { get; }

        public IList<BattleParticipantSnapshot> FriendMain { get; protected set; }
        public IList<BattleParticipantSnapshot> FriendEscort { get; protected set; }

        public IList<BattleParticipantSnapshot> Enemy { get; protected set; }

        internal BattleParticipantSnapshot[] FriendAndEnemy { get; private set; }

        internal protected BattleStage(BattleInfo rpOwner)
        {
            Owner = rpOwner;
        }

        internal void Process(ApiData rpData)
        {
            foreach (var rPhase in Phases)
                rPhase.Process();
        }
    }
}
