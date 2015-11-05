using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/kdock")]
    class ConstructionDockParser : ApiParser<RawConstructionDock[]>
    {
        public override void Process(RawConstructionDock[] rpData) => Game.Port.UpdateConstructionDocks(rpData);
    }
}
