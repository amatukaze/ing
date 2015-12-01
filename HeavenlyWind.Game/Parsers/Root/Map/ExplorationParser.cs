using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/next")]
    class ExplorationParser : ApiParser<RawMapExploration>
    {
        public override void Process(RawMapExploration rpData)
        {
        }
    }
}
