using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Map
{
    [Api("api_req_map/start")]
    class StartSortieParser : ApiParser<RawMapExploration>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawMapExploration rpData)
        {
            var rFleet = Game.Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];
            var rAreaID = int.Parse(rpInfo.Parameters["api_maparea_id"]);
            var rAreaSubID = int.Parse(rpInfo.Parameters["api_mapinfo_no"]);

            var rSortie = new SortieInfo(rpInfo.Timestamp, rFleet, rAreaID * 10 + rAreaSubID);
            Game.Sortie = rSortie;

            var rMap = rSortie.Map;
            if (!rMap.IsEventMap)
                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Sortie,
                    rFleet.ID, rFleet.Name, rMap.MasterInfo.TranslatedName, rAreaID, rAreaSubID));
            else
            {
                if (rMap.HP.Current == 9999 && rMap.HP.Maximum == 9999)
                    rMap.HP.Set(rpData.EventMap.Maximum, rpData.EventMap.Current);

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
                    rFleet.ID, rFleet.Name, rMap.MasterInfo.TranslatedName, rAreaSubID, rDifficulty));
            }

            rSortie.Explore(rpData);
        }
    }
}
