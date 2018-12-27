using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class EnemyAmbushStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.EnemyAmbush;

        public ShellingPhase Shelling { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[] { Shelling };

        internal protected EnemyAmbushStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rDay = rpInfo.Data as RawDay;

            Shelling = new ShellingPhase(this, rDay.ShellingFirstRound);
        }
    }
}
