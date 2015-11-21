using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        static SortieInfo r_Current;

        public Fleet Fleet { get; }
        public MapInfo Map { get; }

        SortieCellInfo r_Cell;
        public SortieCellInfo Cell
        {
            get { return r_Cell; }
            private set
            {
                if (r_Cell != value)
                {
                    r_Cell = value;
                    OnPropertyChanged(nameof(Cell));
                }
            }
        }

        int r_PendingShipCount;
        public int PendingShipCount
        {
            get { return r_PendingShipCount; }
            private set
            {
                if (r_PendingShipCount != value)
                {
                    r_PendingShipCount = value;
                    OnPropertyChanged(nameof(PendingShipCount));
                }
            }
        }

        static SortieInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => r_Current = null);

            Action<ApiData> rExplorationParser = r => r_Current?.Explore(r.Requests, r.GetData<RawMapExploration>());
            SessionService.Instance.Subscribe("api_req_map/start", rExplorationParser);
            SessionService.Instance.Subscribe("api_req_map/next", rExplorationParser);

            Action<ApiData> rProcessIfShipDropped = r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (rData.DroppedShip != null)
                {
                    r_Current.PendingShipCount++;

                    Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Ship_Dropped, rData.DroppedShip.Name));
                }
            };
            SessionService.Instance.Subscribe("api_req_sortie/battleresult", rProcessIfShipDropped);
            SessionService.Instance.Subscribe("api_req_combined_battle/battleresult", rProcessIfShipDropped);

        }
        internal SortieInfo(Fleet rpFleet, int rpMapID)
        {
            r_Current = this;

            Fleet = rpFleet;
            Map = KanColleGame.Current.Maps[rpMapID];
        }

        void Explore(IReadOnlyDictionary<string, string> rpRequests, RawMapExploration rpData)
        {
            Cell = new SortieCellInfo(rpData);

            var rDifficulty = Map.Difficulty;
            if (rDifficulty.HasValue)
            {
                var rDifficultyCount = Enum.GetNames(typeof(EventMapDifficulty)).Length - 1;
                Cell.InternalID = Cell.ID * rDifficultyCount  + (int)rDifficulty.Value - 3;
            }
        }
    }
}
