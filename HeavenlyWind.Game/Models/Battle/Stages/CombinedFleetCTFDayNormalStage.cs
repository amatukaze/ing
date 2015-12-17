using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class CombinedFleetCTFDayNormalStage : CombinedFleetDay
    {
        public override BattleStageType Type => BattleStageType.Day;

        public override IList<BattlePhase> Phases => new List<BattlePhase>()
        {
            AerialCombat,
            SupportingFire,
            OpeningTorpedo,

            ShellingFirstRound,
            ClosingTorpedo,

            ShellingSecondRound,
            ShellingThirdRound,
        }.AsReadOnly();

        internal protected CombinedFleetCTFDayNormalStage(BattleInfo rpOwner, ApiData rpData) : base(rpOwner)
        {
            var rRawData = rpData.Data as RawCombinedFleetDay;

            AerialCombat = new AerialCombatPhase(this, rRawData.AerialCombat);
            SupportingFire = new SupportingFirePhase(this, rRawData.SupportingFire);
            OpeningTorpedo = new TorpedoSalvoPhase(this, rRawData.OpeningTorpedoSalvo, true);

            ShellingFirstRound = new ShellingPhase(this, rRawData.ShellingFirstRound, true);
            ClosingTorpedo = new TorpedoSalvoPhase(this, rRawData.ClosingTorpedoSalvo, true);

            ShellingSecondRound = new ShellingPhase(this, rRawData.ShellingSecondRound);
            ShellingThirdRound = new ShellingPhase(this, rRawData.ShellingThirdRound);
        }
    }
}
