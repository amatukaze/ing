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

        public override IList<BattlePhase> Phases => new List<BattlePhase>()
        {
            AerialCombatFirstRound,
            AerialCombatSecondRound,
        }.AsReadOnly();

        internal protected AerialCombatStage(BattleInfo rpOwner, ApiData rpData) : base(rpOwner)
        {
            var rRawData = rpData.Data as RawAerialCombat;

            AerialCombatFirstRound = new AerialCombatPhase(this, rRawData.AerialCombat, PhaseRound.First);
            AerialCombatSecondRound = new AerialCombatPhase(this, rRawData.AerialCombatSecondRound, PhaseRound.Second);
        }
    }
}
