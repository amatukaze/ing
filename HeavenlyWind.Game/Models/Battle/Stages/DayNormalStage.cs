using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class DayNormalStage : Day
    {
        public override BattleStageType Type => BattleStageType.Day;

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            LandBaseAerialSupport,
            AerialCombat,
            SupportingFire,
            OpeningASW,
            OpeningTorpedo,

            ShellingFirstRound,
            ShellingSecondRound,

            ClosingTorpedo,
        };

        internal protected DayNormalStage(BattleInfo rpOwner, ApiData rpData) : base(rpOwner)
        {
            var rRawData = rpData.Data as RawDay;

            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rRawData.LandBaseAerialSupport);
            AerialCombat = new AerialCombatPhase(this, rRawData.AerialCombat);
            SupportingFire = new SupportingFirePhase(this, rRawData.SupportingFire);
            OpeningASW = new OpeningASWPhase(this, rRawData.OpeningASW);
            OpeningTorpedo = new TorpedoSalvoPhase(this, rRawData.OpeningTorpedoSalvo);

            ShellingFirstRound = new ShellingPhase(this, rRawData.ShellingFirstRound);
            ShellingSecondRound = new ShellingPhase(this, rRawData.ShellingSecondRound);

            ClosingTorpedo = new TorpedoSalvoPhase(this, rRawData.ClosingTorpedoSalvo);
        }
    }
}
