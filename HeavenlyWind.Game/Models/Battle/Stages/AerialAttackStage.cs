using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class AerialAttackStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.AerialAttack;

        public AerialCombatPhase AerialAttack { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[] { AerialAttack };

        internal protected AerialAttackStage(BattleInfo rpOwner, ApiData rpData) : base(rpOwner)
        {
            var rRawData = rpData.Data as IAerialCombat;

            AerialAttack = new AerialCombatPhase(this, rRawData.AerialCombat, PhaseRound.First);
        }
    }
}
