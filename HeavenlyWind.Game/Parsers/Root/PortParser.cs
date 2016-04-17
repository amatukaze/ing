using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root
{
    [Api("api_port/port")]
    class PortParser : ApiParser<RawPort>
    {
        public override void Process(RawPort rpData)
        {
            Game.Port.UpdateAdmiral(rpData.Basic);
            Game.Port.UpdatePort(rpData);
        }
    }
}
