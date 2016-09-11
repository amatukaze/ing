using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class AerialCombatStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.AerialCombat;

        public AerialCombatPhase AerialCombatFirstRound { get; protected set; }
        public AerialCombatPhase AerialCombatSecondRound { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            AerialCombatFirstRound,
            AerialCombatSecondRound,
        };

        internal protected AerialCombatStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as IAerialCombatSecondRound;

            AerialCombatFirstRound = new AerialCombatPhase(this, rRawData.AerialCombat, PhaseRound.First);
            AerialCombatSecondRound = new AerialCombatPhase(this, rRawData.AerialCombatSecondRound, PhaseRound.Second);
        }
    }
}
