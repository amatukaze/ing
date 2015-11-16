using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/start")]
    class StartSortieParser : ApiParser<RawMapExploration>
    {
        public override void Process(RawMapExploration rpData)
        {
            var rFleet = Game.Port.Fleets[int.Parse(Requests["api_deck_id"])];
            var rAreaID = int.Parse(Requests["api_maparea_id"]);
            var rAreaSubID = int.Parse(Requests["api_mapinfo_no"]);

            Game.Sortie = new SortieInfo(rFleet, rAreaID * 10 + rAreaSubID);

            Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Sortie,
                rFleet.ID, rFleet.Name, Game.Sortie.Map.Name, rAreaID, rAreaSubID));
        }
    }
}
