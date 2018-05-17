using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class NightNormalStage : Night
    {
        public override BattleStageType Type => BattleStageType.Night;

        internal protected NightNormalStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as RawNight;

            Shelling = new ShellingPhase(this, rRawData.Shelling);
        }
    }
}
