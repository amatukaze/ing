using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/slot_item")]
    class EquipmentParser : ApiParser<RawEquipment[]>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawEquipment[] rpData)
        {
            Game.Port.UpdateEquipment(rpData);
        }
    }
}
