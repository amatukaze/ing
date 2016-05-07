using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/start_air_base")]
    class LandBaseAerialSupportDesignationParser : ApiParser
    {
        public override void Process()
        {
            var rSortie = SortieInfo.Current;
            if (rSortie == null)
                return;

            rSortie.LandBaseAerialSupportRequests = Parameters.Where(r => r.Key.StartsWith("api_strike_point_")).SelectMany(r => r.Value.Split(',').Select(int.Parse)).Distinct().ToArray();
        }
    }
}
