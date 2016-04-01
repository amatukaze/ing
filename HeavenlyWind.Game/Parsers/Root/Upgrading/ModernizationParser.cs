using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Upgrading
{
    [Api("api_req_kaisou/powerup")]
    class ModernizationParser : ApiParser<RawModernizationResult>
    {
        public override void Process(RawModernizationResult rpData)
        {
        }
    }
}
