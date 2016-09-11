using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_get_member/mapinfo")]
    class GetInformationParser : ApiParser<RawMapInfo[]>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawMapInfo[] rpData)
        {
        }
    }
}
