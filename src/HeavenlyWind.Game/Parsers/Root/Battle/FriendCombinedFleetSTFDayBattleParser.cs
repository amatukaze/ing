using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/battle_water")]
    class FriendCombinedFleetSTFDayBattleParser : ApiParser<RawCombinedFleetDay>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawCombinedFleetDay rpData)
        {
        }
    }
}
