using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_battle_midnight/sp_midnight")]
    class NightOnlyBattleParser : ApiParser<RawNightOnly>
    {
        public override void Process(RawNightOnly rpData)
        {
        }
    }
}
