using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/start_air_base")]
    class LandBaseAerialSupportDesignationParser : ApiParser
    {
        public override void ProcessCore(ApiInfo rpInfo)
        {
            var rSortie = SortieInfo.Current;
            if (rSortie == null)
                return;

            var rRequests = rpInfo.Parameters
                .Where(r => r.Key.StartsWith("api_strike_point_"))
                .SelectMany(r => r.Value.Split(',').Select(int.Parse))
                .Distinct().ToArray();

            var rMap = rSortie.Map.ID;

            for (var i = 0; i < rRequests.Length; i++)
            {
                var rNodeUniqueID = MapService.Instance.GetNodeUniqueID(rMap, rRequests[i]);
                if (!rNodeUniqueID.HasValue)
                    continue;

                rRequests[i] = rNodeUniqueID.Value;
            }

            rSortie.LandBaseAerialSupportRequests = rRequests;
        }
    }
}
