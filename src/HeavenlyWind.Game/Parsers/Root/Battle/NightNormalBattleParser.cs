using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_battle_midnight/battle")]
    class NightNormalBattleParser : ApiParser<RawNight>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawNight rpData)
        {
        }
    }
}
