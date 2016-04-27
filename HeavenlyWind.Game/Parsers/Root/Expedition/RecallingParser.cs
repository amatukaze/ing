using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Expedition
{
    [Api("api_req_mission/return_instruction")]
    public class RecallingParser : ApiParser<RawExpeditionRecalling>
    {
        public override void Process(RawExpeditionRecalling rpData)
        {
        }
    }
}
