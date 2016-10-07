using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class EnemyCombinedFleetDay : CombinedFleetDay
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
            ClosingTorpedo,

            ShellingSecondRound,
            ShellingThirdRound,
        };

        internal protected EnemyCombinedFleetDay(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as RawEnemyCombinedFleetDay;

            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rRawData.LandBaseAerialSupport);
            AerialCombat = new AerialCombatPhase(this, rRawData.AerialCombat);
            SupportingFire = new SupportingFirePhase(this, rRawData.SupportingFire);
            OpeningASW = new OpeningASWPhase(this, rRawData.OpeningASW, true);
            OpeningTorpedo = new TorpedoSalvoPhase(this, rRawData.OpeningTorpedoSalvo, true);

            ShellingFirstRound = new ShellingPhase(this, rRawData.ShellingFirstRound, rpIsEnemyEscortFleet: true);
            ClosingTorpedo = new TorpedoSalvoPhase(this, rRawData.ClosingTorpedoSalvo, true);

            ShellingSecondRound = new ShellingPhase(this, rRawData.ShellingSecondRound);
            ShellingThirdRound = new ShellingPhase(this, rRawData.ShellingThirdRound);
        }
    }
}
