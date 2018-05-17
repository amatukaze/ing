using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class AerialAttackStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.AerialAttack;

        public LandBaseJetAircraftAerialSupport LandBaseJetAircraftAerialSupport { get; protected set; }
        public AerialCombatPhase JetAircraftAerialCombat { get; protected set; }
        public LandBaseAerialSupportPhase LandBaseAerialSupport { get; protected set; }
        public AerialCombatPhase AerialAttack { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            LandBaseJetAircraftAerialSupport,
            JetAircraftAerialCombat,
            LandBaseAerialSupport,
            AerialAttack,
        };

        internal protected AerialAttackStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as IAerialCombat;
            var rDay = rpInfo.Data as RawDay;

            LandBaseJetAircraftAerialSupport = new LandBaseJetAircraftAerialSupport(this, rDay.LandBaseJetAircraftAerialSupport);
            JetAircraftAerialCombat = new AerialCombatPhase(this, rDay.JetAircraftAerialCombat);
            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rDay.LandBaseAerialSupport);
            AerialAttack = new AerialCombatPhase(this, rRawData.AerialCombat, PhaseRound.First);
        }
    }
}
