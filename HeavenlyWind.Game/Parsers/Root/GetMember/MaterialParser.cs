using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/material")]
    class MaterialParser : ApiParser<RawMaterial[]>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawMaterial[] rpData) => Game.Port.Materials.Update(rpData);
    }
}
