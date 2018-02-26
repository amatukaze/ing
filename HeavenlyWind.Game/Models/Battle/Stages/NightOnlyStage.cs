using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class NightOnlyStage : Night
    {
        public override BattleStageType Type => BattleStageType.Night;

        public SupportingFirePhase SupportingFire { get; protected set; }

        public override IList<BattlePhase> Phases => new BattlePhase[]
        {
            SupportingFire,

            Shelling,
        };

        internal protected NightOnlyStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as RawNightOnly;

            Shelling = new ShellingPhase(this, rRawData.Shelling);
            SupportingFire = new SupportingFirePhase(this, rRawData.SupportingFire);
        }
    }
}
