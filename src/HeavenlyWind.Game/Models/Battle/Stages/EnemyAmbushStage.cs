using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class EnemyAmbushStage : BattleStage
    {
        public override BattleStageType Type => BattleStageType.EnemyAmbush;

        public LandBaseJetAircraftAerialSupport LandBaseJetAircraftAerialSupport { get; protected set; }
        public LandBaseAerialSupportPhase LandBaseAerialSupport { get; protected set; }
        public ShellingPhase Shelling { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            LandBaseJetAircraftAerialSupport,
            LandBaseAerialSupport,
            Shelling,
        };

        internal protected EnemyAmbushStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rDay = rpInfo.Data as RawDay;

            LandBaseJetAircraftAerialSupport = new LandBaseJetAircraftAerialSupport(this, rDay.LandBaseJetAircraftAerialSupport);
            LandBaseAerialSupport = new LandBaseAerialSupportPhase(this, rDay.LandBaseAerialSupport);
            Shelling = new ShellingPhase(this, rDay.ShellingFirstRound);
        }
    }
}
