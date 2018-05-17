using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Upgrade
{
    [Api("api_req_kaisou/powerup")]
    class ModernizationParser : ApiParser<RawModernizationResult>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawModernizationResult rpData)
        {
        }
    }
}
