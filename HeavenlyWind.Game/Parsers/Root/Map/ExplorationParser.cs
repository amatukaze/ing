using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/next")]
    class ExplorationParser : ApiParser<RawMapExploration>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawMapExploration rpData)
        {
            var rSortie = Game.Sortie;
            rSortie.Explore(rpInfo.Timestamp, rpData);

            if (rSortie.Node.IsDeadEnd && rSortie.Map.HasGauge && rpData.EventMap != null)
                rSortie.Map.HP.Current = rpData.EventMap.Current;
        }
    }
}
