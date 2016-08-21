using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    abstract class Night : BattleStage
    {
        public ShellingPhase Shelling { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[] { Shelling };

        internal protected Night(BattleInfo rpOwner) : base(rpOwner) { }
    }
}
