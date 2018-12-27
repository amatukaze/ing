using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/ld_shooting")]
    class FriendCombinedFleetEnemyAmbushParser : ApiParser<RawDay>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawDay rpData)
        {
        }
    }
}
