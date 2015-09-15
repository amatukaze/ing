using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/basic")]
    class BasicParser : ApiParser<RawBasic>
    {
        public override void Process(RawBasic rpData)
        {
            Game.Port.UpdateAdmiral(rpData);
        }
    }
}
