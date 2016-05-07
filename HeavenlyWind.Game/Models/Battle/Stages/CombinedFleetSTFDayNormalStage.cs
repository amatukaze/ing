using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class CombinedFleetSTFDayNormalStage : CombinedFleetDay
    {
        public override BattleStageType Type => BattleStageType.Day;

        public override IList<BattlePhase> Phases => new List<BattlePhase>()
        {
            LandBaseAerialSupport,
            AerialCombat,
            SupportingFire,
            OpeningTorpedo,

            ShellingFirstRound,
            ShellingSecondRound,

            ShellingThirdRound,

            ClosingTorpedo,
        }.AsReadOnly();

        internal protected CombinedFleetSTFDayNormalStage(BattleInfo rpOwner, ApiData rpData) : base(rpOwner)
        {
            var rRawData = rpData.Data as RawCombinedFleetDay;

            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rRawData.LandBaseAerialSupport);
            AerialCombat = new AerialCombatPhase(this, rRawData.AerialCombat);
            SupportingFire = new SupportingFirePhase(this, rRawData.SupportingFire);
            OpeningTorpedo = new TorpedoSalvoPhase(this, rRawData.OpeningTorpedoSalvo, true);

            ShellingFirstRound = new ShellingPhase(this, rRawData.ShellingFirstRound);
            ShellingSecondRound = new ShellingPhase(this, rRawData.ShellingSecondRound);

            ShellingThirdRound = new ShellingPhase(this, rRawData.ShellingThirdRound, true);

            ClosingTorpedo = new TorpedoSalvoPhase(this, rRawData.ClosingTorpedoSalvo, true);
        }
    }
}
