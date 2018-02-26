using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Phases;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages
{
    class FriendCombinedFleetNightNormalStage : CombinedFleetNight
    {
        internal protected FriendCombinedFleetNightNormalStage(BattleInfo rpOwner, ApiInfo rpInfo) : base(rpOwner)
        {
            var rRawData = rpInfo.Data as RawCombinedFleetNight;

            Shelling = new ShellingPhase(this, rRawData.Shelling);
        }
    }
}
