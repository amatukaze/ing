using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.GetMember
{
    [Api("api_get_member/base_air_corps")]
    class AirForceGroupParser : ApiParser<RawAirForceGroup[]>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawAirForceGroup[] rpData)
        {
            Game.Port.AirBase.UpdateGroups(rpData);
        }
    }
}
