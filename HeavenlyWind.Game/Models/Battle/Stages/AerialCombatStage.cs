using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class AerialCombatStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.AerialCombat;

        public LandBaseJetAircraftAerialSupport LandBaseJetAircraftAerialSupport { get; protected set; }
        public AerialCombatPhase JetAircraftAerialCombat { get; protected set; }
        public LandBaseAerialSupportPhase LandBaseAerialSupport { get; protected set; }
        public AerialCombatPhase AerialCombatFirstRound { get; protected set; }
        public AerialCombatPhase AerialCombatSecondRound { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            LandBaseJetAircraftAerialSupport,
            JetAircraftAerialCombat,
            LandBaseAerialSupport,
            AerialCombatFirstRound,
            AerialCombatSecondRound,
        };

        internal protected AerialCombatStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as IAerialCombatSecondRound;
            var rDay = rpInfo.Data as RawDay;

            LandBaseJetAircraftAerialSupport = new LandBaseJetAircraftAerialSupport(this, rDay.LandBaseJetAircraftAerialSupport);
            JetAircraftAerialCombat = new AerialCombatPhase(this, rDay.JetAircraftAerialCombat);
            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rDay.LandBaseAerialSupport);
            AerialCombatFirstRound = new AerialCombatPhase(this, rRawData.AerialCombat, PhaseRound.First);
            AerialCombatSecondRound = new AerialCombatPhase(this, rRawData.AerialCombatSecondRound, PhaseRound.Second);
        }
    }
}
