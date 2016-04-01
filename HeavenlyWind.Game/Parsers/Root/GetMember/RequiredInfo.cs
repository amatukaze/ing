using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/require_info")]
    class RequiredInfo : ApiParser<RawRequiredInfo>
    {
        public override void Process(RawRequiredInfo rpData)
        {
            Game.Port.UpdateAdmiral(rpData.Admiral);

            Game.Port.UpdateEquipment(rpData.Equipment);
            Game.Port.UpdateConstructionDocks(rpData.ConstructionDocks);
        }
    }
}
