using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/kdock")]
    class BuildingDockParser : ApiParser<RawBuildingDock[]>
    {
        public override void Process(RawBuildingDock[] rpData) => Game.Port.UpdateBuildingDocks(rpData);
    }
}
