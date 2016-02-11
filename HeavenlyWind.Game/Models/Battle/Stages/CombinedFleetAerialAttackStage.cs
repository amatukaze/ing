using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class CombinedFleetAerialAttackStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.AerialAttack;

        public AerialCombatPhase AerialAttack { get; protected set; }

        public override IList<BattlePhase> Phases => new List<BattlePhase>() { AerialAttack }.AsReadOnly();

        internal protected CombinedFleetAerialAttackStage(BattleInfo rpOwner, ApiData rpData) : base(rpOwner)
        {
            var rRawData = rpData.Data as RawAerialAttack;

            AerialAttack = new AerialCombatPhase(this, rRawData.AerialCombat, PhaseRound.First);
        }
    }
}
