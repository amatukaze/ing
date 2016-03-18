using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/start")]
    class StartSortieParser : ApiParser<RawMapExploration>
    {
        public override void Process(RawMapExploration rpData)
        {
            var rFleet = Game.Port.Fleets[int.Parse(Parameters["api_deck_id"])];
            var rAreaID = int.Parse(Parameters["api_maparea_id"]);
            var rAreaSubID = int.Parse(Parameters["api_mapinfo_no"]);

            Game.Sortie = new SortieInfo(rFleet, rAreaID * 10 + rAreaSubID);

            var rMap = Game.Sortie.Map;
            if (!rMap.IsEventMap)
                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Sortie,
                    rFleet.ID, rFleet.Name, rMap.MasterInfo.Name, rAreaID, rAreaSubID));
            else
            {
                var rDifficulty = string.Empty;
                switch (rMap.Difficulty.Value)
                {
                    case EventMapDifficulty.Easy:
                        rDifficulty = StringResources.Instance.Main.Map_Difficulty_Easy;
                        break;

                    case EventMapDifficulty.Normal:
                        rDifficulty = StringResources.Instance.Main.Map_Difficulty_Normal;
                        break;

                    case EventMapDifficulty.Hard:
                        rDifficulty = StringResources.Instance.Main.Map_Difficulty_Hard;
                        break;
                }

                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Sortie_Event,
                    rFleet.ID, rFleet.Name, rMap.MasterInfo.Name, rAreaSubID, rDifficulty));
            }
        }
    }
}
