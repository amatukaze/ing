using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Arsenal
{
    [Api("api_req_kousyou/getship")]
    class ConstructionResultParser : ApiParser<RawConstructionResult>
    {
        public override void Process(RawConstructionResult rpData)
        {
        }
    }
}
